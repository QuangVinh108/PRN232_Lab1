using PRN232.Lab1.Repositories.IRepository;
using PRN232.Lab1.Services.BusinessModels;
using PRN232.Lab1.Services.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.Lab1.Services.Service
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _repository;

        public StudentService(IStudentRepository repository)
        {
            _repository = repository;
        }

        public async Task<(IEnumerable<StudentBusinessModel> Items, int TotalCount)> GetAllStudentsAsync(string? search, string? sort, int page, int size, string? expand)
        {
            var result = await _repository.GetAllStudentsAsync(search, sort, page, size, expand);

            // Map sang Business Model
            var mappedItems = result.Items.Select(s => new StudentBusinessModel
            {
                StudentId = s.StudentId,
                FullName = s.FullName,
                Email = s.Email,
                DateOfBirth = s.DateOfBirth,
                // Map Enrollments nếu đã được Include (Expand=enrollments)
                Enrollments = s.Enrollments?.Select(e => new EnrollmentBusinessModel
                {
                    EnrollmentId = e.EnrollmentId,
                    StudentId = e.StudentId,
                    CourseId = e.CourseId,
                    EnrollDate = e.EnrollDate,
                    Status = e.Status
                }).ToList()
            });

            return (mappedItems, result.TotalCount);
        }

        public async Task<StudentBusinessModel> GetStudentByIdAsync(int id, string? expand = null)
        {
            var s = await _repository.GetStudentByIdAsync(id, expand);
            if (s == null) return null;
 
            return new StudentBusinessModel
            {
                StudentId = s.StudentId,
                FullName = s.FullName,
                Email = s.Email,
                DateOfBirth = s.DateOfBirth,
                Enrollments = s.Enrollments?.Select(e => new EnrollmentBusinessModel
                {
                    EnrollmentId = e.EnrollmentId,
                    StudentId = e.StudentId,
                    CourseId = e.CourseId,
                    EnrollDate = e.EnrollDate,
                    Status = e.Status
                }).ToList()
            };
        }

        public async Task<StudentBusinessModel> CreateStudentAsync(StudentBusinessModel model)
        {
            var student = new Repositories.Entities.Student
            {
                FullName = model.FullName,
                Email = model.Email,
                DateOfBirth = model.DateOfBirth
            };

            var created = await _repository.CreateStudentAsync(student);
            model.StudentId = created.StudentId;
            return model;
        }

        public async Task<bool> UpdateStudentAsync(int id, StudentBusinessModel model)
        {
            var student = await _repository.GetStudentByIdAsync(id);
            if (student == null) return false;

            student.FullName = model.FullName;
            student.Email = model.Email;
            student.DateOfBirth = model.DateOfBirth;

            await _repository.UpdateStudentAsync(student);
            return true;
        }

        public async Task<bool> DeleteStudentAsync(int id)
        {
            var student = await _repository.GetStudentByIdAsync(id);
            if (student == null) return false;

            await _repository.DeleteStudentAsync(id);
            return true;
        }
    }
}
