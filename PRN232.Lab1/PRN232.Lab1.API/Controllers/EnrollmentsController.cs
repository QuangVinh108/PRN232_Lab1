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
                Status = e.Status
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEnrollmentById(int id)
        {
            var enrollment = await _enrollmentService.GetEnrollmentByIdAsync(id);

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
                Status = enrollment.Status
            };

            return Ok(ApiResponse<EnrollmentResponse>.Ok(responseModel));
        }

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
