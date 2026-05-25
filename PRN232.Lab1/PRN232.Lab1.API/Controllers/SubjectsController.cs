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
    public class SubjectsController : ControllerBase
    {
        private readonly ISubjectService _subjectService;

        public SubjectsController(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        /// <summary>
        /// Get a paginated list of subjects with optional search, sorting, filtering, and relation expansion
        /// </summary>
        /// <param name="queryParams">The query parameter options for search, sort, page, size, fields, and expand</param>
        [HttpGet]
        public async Task<IActionResult> GetAllSubjects([FromQuery] SubjectQueryParameters queryParams)
        {
            var result = await _subjectService.GetAllSubjectsAsync(
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

            var responseModels = result.Items.Select(s => new SubjectResponse
            {
                SubjectId = s.SubjectId,
                SubjectCode = s.SubjectCode,
                SubjectName = s.SubjectName,
                Credit = s.Credit
            });

            if (!string.IsNullOrWhiteSpace(queryParams.Fields))
            {
                var fields = queryParams.Fields.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(f => f.Trim().ToLower());
                var shapedData = new List<ExpandoObject>();

                foreach (var item in responseModels)
                {
                    var shapedObj = new ExpandoObject() as IDictionary<string, object>;
                    var properties = typeof(SubjectResponse).GetProperties(BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

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

            return Ok(ApiResponse<IEnumerable<SubjectResponse>>.Ok(responseModels, metadata));
        }

        /// <summary>
        /// Get subject details by ID
        /// </summary>
        /// <param name="id">ID of the subject to retrieve</param>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSubjectById(int id)
        {
            var subject = await _subjectService.GetSubjectByIdAsync(id);

            if (subject == null)
            {
                return NotFound(ApiResponse<SubjectResponse>.NotFound($"Subject with ID {id} not found."));
            }

            var responseModel = new SubjectResponse
            {
                SubjectId = subject.SubjectId,
                SubjectCode = subject.SubjectCode,
                SubjectName = subject.SubjectName,
                Credit = subject.Credit
            };

            return Ok(ApiResponse<SubjectResponse>.Ok(responseModel));
        }

        /// <summary>
        /// Create a new subject
        /// </summary>
        /// <param name="request">The subject creation request body</param>
        [HttpPost]
        public async Task<IActionResult> CreateSubject([FromBody] SubjectRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var model = new Services.BusinessModels.SubjectBusinessModel
            {
                SubjectCode = request.SubjectCode,
                SubjectName = request.SubjectName,
                Credit = request.Credit
            };

            var created = await _subjectService.CreateSubjectAsync(model);

            var response = new SubjectResponse
            {
                SubjectId = created.SubjectId,
                SubjectCode = created.SubjectCode,
                SubjectName = created.SubjectName,
                Credit = created.Credit
            };

            return CreatedAtAction(nameof(GetSubjectById), new { id = response.SubjectId }, ApiResponse<SubjectResponse>.Ok(response));
        }

        /// <summary>
        /// Update an existing subject by ID
        /// </summary>
        /// <param name="id">ID of the subject to update</param>
        /// <param name="request">The subject update request body</param>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSubject(int id, [FromBody] SubjectRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var model = new Services.BusinessModels.SubjectBusinessModel
            {
                SubjectCode = request.SubjectCode,
                SubjectName = request.SubjectName,
                Credit = request.Credit
            };

            var isUpdated = await _subjectService.UpdateSubjectAsync(id, model);

            if (!isUpdated)
            {
                return NotFound(ApiResponse<string>.NotFound($"Subject with ID {id} not found."));
            }

            return NoContent();
        }

        /// <summary>
        /// Delete an existing subject by ID
        /// </summary>
        /// <param name="id">ID of the subject to delete</param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubject(int id)
        {
            var isDeleted = await _subjectService.DeleteSubjectAsync(id);

            if (!isDeleted)
            {
                return NotFound(ApiResponse<string>.NotFound($"Subject with ID {id} not found."));
            }

            return NoContent();
        }
    }
}
