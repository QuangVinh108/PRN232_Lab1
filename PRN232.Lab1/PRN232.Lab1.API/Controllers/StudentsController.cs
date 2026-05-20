using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PRN232.Lab1.API.RequestModels;
using PRN232.Lab1.API.ResponseModels;
using PRN232.Lab1.Services.IService;
using System.Dynamic;
using System.Reflection;

namespace PRN232.Lab1.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;

        // Tiêm IStudentService vào Controller
        public StudentsController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        // GET: api/students
        [HttpGet]
        public async Task<IActionResult> GetAllStudents([FromQuery] StudentQueryParameters queryParams)
        {
            // 1. Lấy dữ liệu từ Service (Nhớ truyền thêm queryParams.Expand)
            var result = await _studentService.GetAllStudentsAsync(
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

            // 2. Map sang Response Model
            var responseModels = result.Items.Select(s => new StudentResponse
            {
                StudentId = s.StudentId,
                FullName = s.FullName,
                Email = s.Email,
                DateOfBirth = s.DateOfBirth,
                // Nếu s.Enrollments có dữ liệu (do Expansion), map nó sang, nếu không thì để null
                Enrollments = s.Enrollments?.Select(e => new EnrollmentResponse
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

                foreach (var student in responseModels)
                {
                    var shapedObj = new ExpandoObject() as IDictionary<string, object>;
                    var properties = typeof(StudentResponse).GetProperties(BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                    foreach (var property in properties)
                    {
                        if (fields.Contains(property.Name.ToLower()))
                        {
                            shapedObj.Add(property.Name, property.GetValue(student));
                        }
                    }
                    shapedData.Add((ExpandoObject)shapedObj);
                }

                return Ok(ApiResponse<IEnumerable<dynamic>>.Ok(shapedData, metadata));
            }

            // Nếu không có yêu cầu Selection, trả về nguyên bản
            return Ok(ApiResponse<IEnumerable<StudentResponse>>.Ok(responseModels, metadata));
        }

        // GET: api/students/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetStudentById(int id)
        {
            // 1. Lấy dữ liệu từ Service (Trả về StudentBusinessModel)
            var student = await _studentService.GetStudentByIdAsync(id);

            // 2. Trả về 404 Not Found nếu không có dữ liệu
            if (student == null)
            {
                return NotFound(ApiResponse<StudentResponse>.NotFound($"Student with ID {id} not found."));
            }

            // 3. Map từ Business Model sang Response Model
            var responseModel = new StudentResponse
            {
                StudentId = student.StudentId,
                FullName = student.FullName,
                Email = student.Email,
                DateOfBirth = student.DateOfBirth
            };

            // 4. Trả về 200 OK nếu tìm thấy
            return Ok(ApiResponse<StudentResponse>.Ok(responseModel));
        }

        // POST: api/students
        [HttpPost]
        public async Task<IActionResult> CreateStudent([FromBody] StudentRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var model = new Services.BusinessModels.StudentBusinessModel
            {
                FullName = request.FullName,
                Email = request.Email,
                DateOfBirth = request.DateOfBirth!.Value
            };

            var createdStudent = await _studentService.CreateStudentAsync(model);
            
            var responseModel = new StudentResponse
            {
                StudentId = createdStudent.StudentId,
                FullName = createdStudent.FullName,
                Email = createdStudent.Email,
                DateOfBirth = createdStudent.DateOfBirth
            };

            return CreatedAtAction(nameof(GetStudentById), new { id = responseModel.StudentId }, ApiResponse<StudentResponse>.Ok(responseModel));
        }

        // PUT: api/students/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(int id, [FromBody] StudentRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var model = new Services.BusinessModels.StudentBusinessModel
            {
                FullName = request.FullName,
                Email = request.Email,
                DateOfBirth = request.DateOfBirth!.Value
            };

            var isUpdated = await _studentService.UpdateStudentAsync(id, model);

            if (!isUpdated)
            {
                return NotFound(ApiResponse<string>.NotFound($"Student with ID {id} not found."));
            }

            return NoContent();
        }

        // DELETE: api/students/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var isDeleted = await _studentService.DeleteStudentAsync(id);

            if (!isDeleted)
            {
                return NotFound(ApiResponse<string>.NotFound($"Student with ID {id} not found."));
            }

            return NoContent();
        }
    }
}
