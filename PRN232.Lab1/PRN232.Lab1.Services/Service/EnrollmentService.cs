using PRN232.Lab1.Repositories.IRepository;
using PRN232.Lab1.Services.BusinessModels;
using PRN232.Lab1.Services.IService;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PRN232.Lab1.Services.Service
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IEnrollmentRepository _repository;

        public EnrollmentService(IEnrollmentRepository repository)
        {
            _repository = repository;
        }

        public async Task<(IEnumerable<EnrollmentBusinessModel> Items, int TotalCount)> GetAllEnrollmentsAsync(string? search, string? sort, int page, int size, string? expand)
        {
            var result = await _repository.GetAllEnrollmentsAsync(search, sort, page, size, expand);

            var mappedItems = result.Items.Select(e => new EnrollmentBusinessModel
            {
                EnrollmentId = e.EnrollmentId,
                StudentId = e.StudentId,
                CourseId = e.CourseId,
                EnrollDate = e.EnrollDate,
                Status = e.Status
            });

            return (mappedItems, result.TotalCount);
        }

        public async Task<EnrollmentBusinessModel> GetEnrollmentByIdAsync(int id)
        {
            var e = await _repository.GetEnrollmentByIdAsync(id);
            if (e == null) return null;

            return new EnrollmentBusinessModel
            {
                EnrollmentId = e.EnrollmentId,
                StudentId = e.StudentId,
                CourseId = e.CourseId,
                EnrollDate = e.EnrollDate,
                Status = e.Status
            };
        }

        public async Task<EnrollmentBusinessModel> CreateEnrollmentAsync(EnrollmentBusinessModel model)
        {
            var enrollment = new Repositories.Entities.Enrollment
            {
                StudentId = model.StudentId,
                CourseId = model.CourseId,
                EnrollDate = model.EnrollDate,
                Status = model.Status
            };

            var created = await _repository.CreateEnrollmentAsync(enrollment);
            model.EnrollmentId = created.EnrollmentId;
            return model;
        }

        public async Task<bool> UpdateEnrollmentAsync(int id, EnrollmentBusinessModel model)
        {
            var enrollment = await _repository.GetEnrollmentByIdAsync(id);
            if (enrollment == null) return false;

            enrollment.StudentId = model.StudentId;
            enrollment.CourseId = model.CourseId;
            enrollment.EnrollDate = model.EnrollDate;
            enrollment.Status = model.Status;

            await _repository.UpdateEnrollmentAsync(enrollment);
            return true;
        }

        public async Task<bool> DeleteEnrollmentAsync(int id)
        {
            var enrollment = await _repository.GetEnrollmentByIdAsync(id);
            if (enrollment == null) return false;

            await _repository.DeleteEnrollmentAsync(id);
            return true;
        }
    }
}
