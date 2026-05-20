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
    public class SemestersController : ControllerBase
    {
        private readonly ISemesterService _semesterService;

        public SemestersController(ISemesterService semesterService)
        {
            _semesterService = semesterService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSemesters([FromQuery] SemesterQueryParameters queryParams)
        {
            var result = await _semesterService.GetAllSemestersAsync(
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

            var responseModels = result.Items.Select(s => new SemesterResponse
            {
                SemesterId = s.SemesterId,
                SemesterName = s.SemesterName,
                StartDate = s.StartDate,
                EndDate = s.EndDate
            });

            if (!string.IsNullOrWhiteSpace(queryParams.Fields))
            {
                var fields = queryParams.Fields.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(f => f.Trim().ToLower());
                var shapedData = new List<ExpandoObject>();

                foreach (var item in responseModels)
                {
                    var shapedObj = new ExpandoObject() as IDictionary<string, object>;
                    var properties = typeof(SemesterResponse).GetProperties(BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

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

            return Ok(ApiResponse<IEnumerable<SemesterResponse>>.Ok(responseModels, metadata));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSemesterById(int id)
        {
            var semester = await _semesterService.GetSemesterByIdAsync(id);

            if (semester == null)
            {
                return NotFound(ApiResponse<SemesterResponse>.NotFound($"Semester with ID {id} not found."));
            }

            var responseModel = new SemesterResponse
            {
                SemesterId = semester.SemesterId,
                SemesterName = semester.SemesterName,
                StartDate = semester.StartDate,
                EndDate = semester.EndDate
            };

            return Ok(ApiResponse<SemesterResponse>.Ok(responseModel));
        }

        [HttpPost]
        public async Task<IActionResult> CreateSemester([FromBody] SemesterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var model = new Services.BusinessModels.SemesterBusinessModel
            {
                SemesterName = request.SemesterName,
                StartDate = request.StartDate,
                EndDate = request.EndDate
            };

            var created = await _semesterService.CreateSemesterAsync(model);

            var response = new SemesterResponse
            {
                SemesterId = created.SemesterId,
                SemesterName = created.SemesterName,
                StartDate = created.StartDate,
                EndDate = created.EndDate
            };

            return CreatedAtAction(nameof(GetSemesterById), new { id = response.SemesterId }, ApiResponse<SemesterResponse>.Ok(response));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSemester(int id, [FromBody] SemesterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var model = new Services.BusinessModels.SemesterBusinessModel
            {
                SemesterName = request.SemesterName,
                StartDate = request.StartDate,
                EndDate = request.EndDate
            };

            var isUpdated = await _semesterService.UpdateSemesterAsync(id, model);

            if (!isUpdated)
            {
                return NotFound(ApiResponse<string>.NotFound($"Semester with ID {id} not found."));
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSemester(int id)
        {
            var isDeleted = await _semesterService.DeleteSemesterAsync(id);

            if (!isDeleted)
            {
                return NotFound(ApiResponse<string>.NotFound($"Semester with ID {id} not found."));
            }

            return NoContent();
        }
    }
}
