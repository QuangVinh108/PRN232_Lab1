using PRN232.Lab1.Repositories.IRepository;
using PRN232.Lab1.Services.BusinessModels;
using PRN232.Lab1.Services.IService;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PRN232.Lab1.Services.Service
{
    public class SemesterService : ISemesterService
    {
        private readonly ISemesterRepository _repository;

        public SemesterService(ISemesterRepository repository)
        {
            _repository = repository;
        }

        public async Task<(IEnumerable<SemesterBusinessModel> Items, int TotalCount)> GetAllSemestersAsync(string? search, string? sort, int page, int size, string? expand)
        {
            var result = await _repository.GetAllSemestersAsync(search, sort, page, size, expand);

            var mappedItems = result.Items.Select(s => new SemesterBusinessModel
            {
                SemesterId = s.SemesterId,
                SemesterName = s.SemesterName,
                StartDate = s.StartDate,
                EndDate = s.EndDate,
                Courses = s.Courses?.Select(c => new CourseBusinessModel
                {
                    CourseId = c.CourseId,
                    CourseName = c.CourseName,
                    SemesterId = c.SemesterId
                }).ToList()
            });

            return (mappedItems, result.TotalCount);
        }

        public async Task<SemesterBusinessModel> GetSemesterByIdAsync(int id, string? expand = null)
        {
            var s = await _repository.GetSemesterByIdAsync(id, expand);
            if (s == null) return null;

            return new SemesterBusinessModel
            {
                SemesterId = s.SemesterId,
                SemesterName = s.SemesterName,
                StartDate = s.StartDate,
                EndDate = s.EndDate,
                Courses = s.Courses?.Select(c => new CourseBusinessModel
                {
                    CourseId = c.CourseId,
                    CourseName = c.CourseName,
                    SemesterId = c.SemesterId
                }).ToList()
            };
        }

        public async Task<SemesterBusinessModel> CreateSemesterAsync(SemesterBusinessModel model)
        {
            var semester = new Repositories.Entities.Semester
            {
                SemesterName = model.SemesterName,
                StartDate = model.StartDate,
                EndDate = model.EndDate
            };

            var created = await _repository.CreateSemesterAsync(semester);
            model.SemesterId = created.SemesterId;
            return model;
        }

        public async Task<bool> UpdateSemesterAsync(int id, SemesterBusinessModel model)
        {
            var semester = await _repository.GetSemesterByIdAsync(id);
            if (semester == null) return false;

            semester.SemesterName = model.SemesterName;
            semester.StartDate = model.StartDate;
            semester.EndDate = model.EndDate;

            await _repository.UpdateSemesterAsync(semester);
            return true;
        }

        public async Task<bool> DeleteSemesterAsync(int id)
        {
            var semester = await _repository.GetSemesterByIdAsync(id);
            if (semester == null) return false;

            await _repository.DeleteSemesterAsync(id);
            return true;
        }
    }
}
