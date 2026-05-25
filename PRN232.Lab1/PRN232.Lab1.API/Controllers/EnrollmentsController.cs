using Microsoft.AspNetCore.Mvc;
using PRN232.Lab1.API.RequestModels;
using PRN232.Lab1.API.ResponseModels;
using PRN232.Lab1.Services.IService;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace PRN232.Lab1.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;

        public EnrollmentsController(IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        /// <summary>
        /// Get a paginated list of enrollments with optional search, sorting, filtering, and relation expansion
        /// </summary>
        /// <param name="queryParams">The query parameter options for search, sort, page, size, fields, and expand</param>
        [HttpGet]
        public async Task<IActionResult> GetAllEnrollments([FromQuery] EnrollmentQueryParameters queryParams)
        {
            var result = await _enrollmentService.GetAllEnrollmentsAsync(
                queryParams.Search,
                queryParams.Sort,
                queryParams.Page,
                queryParams.Size,
                queryParams.Expand);

            var totalPages = (int)Math.Ceiling(result.TotalCount / (double)queryParams.Size);
            var metadata = new PaginationMetadata
            {
                Page = queryParams.Page,
                PageSize = queryParams.Size,
                TotalItems = result.TotalCount,
                TotalPages = totalPages
            };

            var responseModels = result.Items.Select(e => new EnrollmentResponse
            {
                EnrollmentId = e.EnrollmentId,
                StudentId = e.StudentId,
                CourseId = e.CourseId,
                EnrollDate = e.EnrollDate,
                Status = e.Status,
                Student = e.Student != null ? new StudentResponse
                {
                    StudentId = e.Student.StudentId,
                    FullName = e.Student.FullName,
                    Email = e.Student.Email,
                    DateOfBirth = e.Student.DateOfBirth
                } : null,
                Course = e.Course != null ? new CourseResponse
                {
                    CourseId = e.Course.CourseId,
                    CourseName = e.Course.CourseName,
                    SemesterId = e.Course.SemesterId
                } : null
            });

            if (!string.IsNullOrWhiteSpace(queryParams.Fields))
            {
                var fields = queryParams.Fields.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(f => f.Trim().ToLower());
                var shapedData = new List<ExpandoObject>();

                foreach (var item in responseModels)
                {
                    var shapedObj = new ExpandoObject() as IDictionary<string, object>;
                    var properties = typeof(EnrollmentResponse).GetProperties(BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                    foreach (var property in properties)
                    {
                        if (fields.Contains(property.Name.ToLower()))
                        {
                            shapedObj.Add(property.Name, property.GetValue(item));
                        }
                    }
                    shapedData.Add((ExpandoObject)shapedObj);
                }

                return Ok(ApiResponse<IEnumerable<dynamic>>.Ok(shapedData, metadata));
            }

            return Ok(ApiResponse<IEnumerable<EnrollmentResponse>>.Ok(responseModels, metadata));
        }

        /// <summary>
        /// Get enrollment details by ID
        /// </summary>
        /// <param name="id">ID of the enrollment to retrieve</param>
        /// <param name="expand">Expand related entities (e.g. 'student' or 'course')</param>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEnrollmentById(int id, [FromQuery] string? expand)
        {
            var enrollment = await _enrollmentService.GetEnrollmentByIdAsync(id, expand);

            if (enrollment == null)
            {
                return NotFound(ApiResponse<EnrollmentResponse>.NotFound($"Enrollment with ID {id} not found."));
            }

            var responseModel = new EnrollmentResponse
            {
                EnrollmentId = enrollment.EnrollmentId,
                StudentId = enrollment.StudentId,
                CourseId = enrollment.CourseId,
                EnrollDate = enrollment.EnrollDate,
                Status = enrollment.Status,
                Student = enrollment.Student != null ? new StudentResponse
                {
                    StudentId = enrollment.Student.StudentId,
                    FullName = enrollment.Student.FullName,
                    Email = enrollment.Student.Email,
                    DateOfBirth = enrollment.Student.DateOfBirth
                } : null,
                Course = enrollment.Course != null ? new CourseResponse
                {
                    CourseId = enrollment.Course.CourseId,
                    CourseName = enrollment.Course.CourseName,
                    SemesterId = enrollment.Course.SemesterId
                } : null
            };

            return Ok(ApiResponse<EnrollmentResponse>.Ok(responseModel));
        }

        /// <summary>
        /// Create a new student enrollment
        /// </summary>
        /// <param name="request">The enrollment creation request body</param>
        [HttpPost]
        public async Task<IActionResult> CreateEnrollment([FromBody] EnrollmentRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var model = new Services.BusinessModels.EnrollmentBusinessModel
            {
                StudentId = request.StudentId,
                CourseId = request.CourseId,
                EnrollDate = request.EnrollDate,
                Status = request.Status
            };

            var created = await _enrollmentService.CreateEnrollmentAsync(model);

            var response = new EnrollmentResponse
            {
                EnrollmentId = created.EnrollmentId,
                StudentId = created.StudentId,
                CourseId = created.CourseId,
                EnrollDate = created.EnrollDate,
                Status = created.Status
            };

            return CreatedAtAction(nameof(GetEnrollmentById), new { id = response.EnrollmentId }, ApiResponse<EnrollmentResponse>.Ok(response));
        }

        /// <summary>
        /// Update an existing enrollment by ID
        /// </summary>
        /// <param name="id">ID of the enrollment to update</param>
        /// <param name="request">The enrollment update request body</param>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEnrollment(int id, [FromBody] EnrollmentRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var model = new Services.BusinessModels.EnrollmentBusinessModel
            {
                StudentId = request.StudentId,
                CourseId = request.CourseId,
                EnrollDate = request.EnrollDate,
                Status = request.Status
            };

            var isUpdated = await _enrollmentService.UpdateEnrollmentAsync(id, model);

            if (!isUpdated)
            {
                return NotFound(ApiResponse<string>.NotFound($"Enrollment with ID {id} not found."));
            }

            return NoContent();
        }

        /// <summary>
        /// Delete an existing enrollment by ID
        /// </summary>
        /// <param name="id">ID of the enrollment to delete</param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEnrollment(int id)
        {
            var isDeleted = await _enrollmentService.DeleteEnrollmentAsync(id);

            if (!isDeleted)
            {
                return NotFound(ApiResponse<string>.NotFound($"Enrollment with ID {id} not found."));
            }

            return NoContent();
        }
    }
}
