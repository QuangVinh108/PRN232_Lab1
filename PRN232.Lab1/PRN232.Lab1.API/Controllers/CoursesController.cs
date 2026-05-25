using Microsoft.AspNetCore.Mvc;
using PRN232.Lab1.API.RequestModels;
using PRN232.Lab1.API.ResponseModels;
using PRN232.Lab1.Services.IService;
using PRN232.Lab1.Services.BusinessModels;
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

        /// <summary>
        /// Get a paginated list of courses with optional search, sorting, filtering, and relation expansion
        /// </summary>
        /// <param name="queryParams">The query parameter options for search, sort, page, size, fields, and expand</param>
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
                Semester = c.Semester != null ? new SemesterResponse
                {
                    SemesterId = c.Semester.SemesterId,
                    SemesterName = c.Semester.SemesterName,
                    StartDate = c.Semester.StartDate,
                    EndDate = c.Semester.EndDate
                } : null,
                Enrollments = c.Enrollments?.Select(e => new EnrollmentResponse
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
                }).ToList(),
                Students = c.Students?.Select(s => new StudentResponse
                {
                    StudentId = s.StudentId,
                    FullName = s.FullName,
                    Email = s.Email,
                    DateOfBirth = s.DateOfBirth
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

        /// <summary>
        /// Get course details by ID
        /// </summary>
        /// <param name="id">ID of the course</param>
        /// <param name="fields">Select specific fields to return, separated by commas (e.g. 'courseId,courseName')</param>
        /// <param name="expand">Expand related entities (e.g. 'students' to load student information or 'semester' to load semester details)</param>
        /// <param name="page">Page number of the expanded relation list, starting from 1</param>
        /// <param name="size">Maximum number of items per page of the expanded relation list</param>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourseById(
            int id, 
            [FromQuery] string? fields, 
            [FromQuery] string? expand,
            [FromQuery] int page = 1,
            [FromQuery] int size = 10)
        {
            var course = await _courseService.GetCourseByIdAsync(id, expand);

            if (course == null)
            {
                return NotFound(ApiResponse<CourseResponse>.NotFound($"Course with ID {id} not found."));
            }

            var responseModel = new CourseResponse
            {
                CourseId = course.CourseId,
                CourseName = course.CourseName,
                SemesterId = course.SemesterId,
                Semester = course.Semester != null ? new SemesterResponse
                {
                    SemesterId = course.Semester.SemesterId,
                    SemesterName = course.Semester.SemesterName,
                    StartDate = course.Semester.StartDate,
                    EndDate = course.Semester.EndDate
                } : null,
                Enrollments = course.Enrollments?.Select(e => new EnrollmentResponse
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
                }).ToList(),
                Students = course.Students?.Select(s => new StudentResponse
                {
                    StudentId = s.StudentId,
                    FullName = s.FullName,
                    Email = s.Email,
                    DateOfBirth = s.DateOfBirth
                }).ToList()
            };

            PaginationMetadata? metadata = null;
            if (!string.IsNullOrWhiteSpace(expand))
            {
                if (expand.Contains("student", StringComparison.OrdinalIgnoreCase) && responseModel.Students != null)
                {
                    var totalItems = responseModel.Students.Count;
                    var paginatedStudents = responseModel.Students
                        .Skip((page - 1) * size)
                        .Take(size)
                        .ToList();
                    
                    responseModel.Students = paginatedStudents;
                    
                    var totalPages = (int)Math.Ceiling(totalItems / (double)size);
                    metadata = new PaginationMetadata
                    {
                        Page = page,
                        PageSize = size,
                        TotalItems = totalItems,
                        TotalPages = totalPages
                    };
                }
                else if (expand.Contains("enrollment", StringComparison.OrdinalIgnoreCase) && responseModel.Enrollments != null)
                {
                    var totalItems = responseModel.Enrollments.Count;
                    var paginatedEnrollments = responseModel.Enrollments
                        .Skip((page - 1) * size)
                        .Take(size)
                        .ToList();
                    
                    responseModel.Enrollments = paginatedEnrollments;
                    
                    var totalPages = (int)Math.Ceiling(totalItems / (double)size);
                    metadata = new PaginationMetadata
                    {
                        Page = page,
                        PageSize = size,
                        TotalItems = totalItems,
                        TotalPages = totalPages
                    };
                }
            }

            if (!string.IsNullOrWhiteSpace(fields))
            {
                var fieldList = fields.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(f => f.Trim().ToLower());
                var shapedObj = new ExpandoObject() as IDictionary<string, object>;
                var properties = typeof(CourseResponse).GetProperties(BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                foreach (var property in properties)
                {
                    if (fieldList.Contains(property.Name.ToLower()))
                    {
                        shapedObj.Add(property.Name, property.GetValue(responseModel));
                    }
                }

                return Ok(ApiResponse<dynamic>.Ok(shapedObj, metadata));
            }

            return Ok(ApiResponse<CourseResponse>.Ok(responseModel, metadata));
        }

        /// <summary>
        /// Get enrollments for a specific course by course ID with optional student expansion
        /// </summary>
        /// <param name="id">ID of the course</param>
        /// <param name="expand">Expand related entities (e.g. 'student')</param>
        [HttpGet("{id}/enrollments")]
        public async Task<IActionResult> GetCourseEnrollments(int id, [FromQuery] string? expand)
        {
            var course = await _courseService.GetCourseByIdAsync(id, "student");

            if (course == null)
            {
                return NotFound(ApiResponse<IEnumerable<EnrollmentResponse>>.NotFound($"Course with ID {id} not found."));
            }

            var enrollments = course.Enrollments ?? new List<EnrollmentBusinessModel>();

            var responseModels = enrollments.Select(e => new EnrollmentResponse
            {
                EnrollmentId = e.EnrollmentId,
                StudentId = e.StudentId,
                CourseId = e.CourseId,
                EnrollDate = e.EnrollDate,
                Status = e.Status,
                Student = (expand != null && expand.Contains("student", StringComparison.OrdinalIgnoreCase) && e.Student != null) 
                    ? new StudentResponse
                    {
                        StudentId = e.Student.StudentId,
                        FullName = e.Student.FullName,
                        Email = e.Student.Email,
                        DateOfBirth = e.Student.DateOfBirth
                    } : null,
                Course = null
            });

            return Ok(ApiResponse<IEnumerable<EnrollmentResponse>>.Ok(responseModels));
        }

        /// <summary>
        /// Create a new course
        /// </summary>
        /// <param name="request">The course creation request body</param>
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

        /// <summary>
        /// Update an existing course by ID
        /// </summary>
        /// <param name="id">ID of the course to update</param>
        /// <param name="request">The course update request body</param>
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

        /// <summary>
        /// Delete an existing course by ID
        /// </summary>
        /// <param name="id">ID of the course to delete</param>
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
