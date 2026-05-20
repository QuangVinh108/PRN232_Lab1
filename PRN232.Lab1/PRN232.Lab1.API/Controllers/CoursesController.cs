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
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CoursesController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCourses([FromQuery] CourseQueryParameters queryParams)
        {
            var result = await _courseService.GetAllCoursesAsync(
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

            var responseModels = result.Items.Select(c => new CourseResponse
            {
                CourseId = c.CourseId,
                CourseName = c.CourseName,
                SemesterId = c.SemesterId,
                Enrollments = c.Enrollments?.Select(e => new EnrollmentResponse
                {
                    EnrollmentId = e.EnrollmentId,
                    CourseId = e.CourseId,
                    EnrollDate = e.EnrollDate,
                    Status = e.Status
                }).ToList()
            });

            if (!string.IsNullOrWhiteSpace(queryParams.Fields))
            {
                var fields = queryParams.Fields.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(f => f.Trim().ToLower());
                var shapedData = new List<ExpandoObject>();

                foreach (var item in responseModels)
                {
                    var shapedObj = new ExpandoObject() as IDictionary<string, object>;
                    var properties = typeof(CourseResponse).GetProperties(BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

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

            return Ok(ApiResponse<IEnumerable<CourseResponse>>.Ok(responseModels, metadata));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourseById(int id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);

            if (course == null)
            {
                return NotFound(ApiResponse<CourseResponse>.NotFound($"Course with ID {id} not found."));
            }

            var responseModel = new CourseResponse
            {
                CourseId = course.CourseId,
                CourseName = course.CourseName,
                SemesterId = course.SemesterId
            };

            return Ok(ApiResponse<CourseResponse>.Ok(responseModel));
        }

        [HttpPost]
        public async Task<IActionResult> CreateCourse([FromBody] CourseRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var model = new Services.BusinessModels.CourseBusinessModel
            {
                CourseName = request.CourseName,
                SemesterId = request.SemesterId
            };

            var created = await _courseService.CreateCourseAsync(model);
            
            var response = new CourseResponse
            {
                CourseId = created.CourseId,
                CourseName = created.CourseName,
                SemesterId = created.SemesterId
            };

            return CreatedAtAction(nameof(GetCourseById), new { id = response.CourseId }, ApiResponse<CourseResponse>.Ok(response));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourse(int id, [FromBody] CourseRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var model = new Services.BusinessModels.CourseBusinessModel
            {
                CourseName = request.CourseName,
                SemesterId = request.SemesterId
            };

            var isUpdated = await _courseService.UpdateCourseAsync(id, model);

            if (!isUpdated)
            {
                return NotFound(ApiResponse<string>.NotFound($"Course with ID {id} not found."));
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var isDeleted = await _courseService.DeleteCourseAsync(id);

            if (!isDeleted)
            {
                return NotFound(ApiResponse<string>.NotFound($"Course with ID {id} not found."));
            }

            return NoContent();
        }
    }
}
