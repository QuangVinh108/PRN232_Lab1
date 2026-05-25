using PRN232.Lab1.Repositories.IRepository;
using PRN232.Lab1.Services.BusinessModels;
using PRN232.Lab1.Services.IService;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PRN232.Lab1.Services.Service
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _repository;

        public CourseService(ICourseRepository repository)
        {
            _repository = repository;
        }

        public async Task<(IEnumerable<CourseBusinessModel> Items, int TotalCount)> GetAllCoursesAsync(string? search, string? sort, int page, int size, string? expand)
        {
            var result = await _repository.GetAllCoursesAsync(search, sort, page, size, expand);

            var mappedItems = result.Items.Select(c => new CourseBusinessModel
            {
                CourseId = c.CourseId,
                CourseName = c.CourseName,
                SemesterId = c.SemesterId,
                Semester = c.Semester != null ? new SemesterBusinessModel
                {
                    SemesterId = c.Semester.SemesterId,
                    SemesterName = c.Semester.SemesterName,
                    StartDate = c.Semester.StartDate,
                    EndDate = c.Semester.EndDate
                } : null,
                Enrollments = c.Enrollments?.Select(e => new EnrollmentBusinessModel
                {
                    EnrollmentId = e.EnrollmentId,
                    StudentId = e.StudentId,
                    CourseId = e.CourseId,
                    EnrollDate = e.EnrollDate,
                    Status = e.Status,
                    Student = e.Student != null ? new StudentBusinessModel
                    {
                        StudentId = e.Student.StudentId,
                        FullName = e.Student.FullName,
                        Email = e.Student.Email,
                        DateOfBirth = e.Student.DateOfBirth
                    } : null,
                    Course = null
                }).ToList(),
                Students = c.Enrollments?.Where(e => e.Student != null).Select(e => new StudentBusinessModel
                {
                    StudentId = e.Student.StudentId,
                    FullName = e.Student.FullName,
                    Email = e.Student.Email,
                    DateOfBirth = e.Student.DateOfBirth
                }).ToList()
            });

            return (mappedItems, result.TotalCount);
        }

        public async Task<CourseBusinessModel> GetCourseByIdAsync(int id, string? expand = null)
        {
            var c = await _repository.GetCourseByIdAsync(id, expand);
            if (c == null) return null;

            return new CourseBusinessModel
            {
                CourseId = c.CourseId,
                CourseName = c.CourseName,
                SemesterId = c.SemesterId,
                Semester = c.Semester != null ? new SemesterBusinessModel
                {
                    SemesterId = c.Semester.SemesterId,
                    SemesterName = c.Semester.SemesterName,
                    StartDate = c.Semester.StartDate,
                    EndDate = c.Semester.EndDate
                } : null,
                Enrollments = c.Enrollments != null && c.Enrollments.Any() ? c.Enrollments.Select(e => new EnrollmentBusinessModel
                {
                    EnrollmentId = e.EnrollmentId,
                    StudentId = e.StudentId,
                    CourseId = e.CourseId,
                    EnrollDate = e.EnrollDate,
                    Status = e.Status,
                    Student = e.Student != null ? new StudentBusinessModel
                    {
                        StudentId = e.Student.StudentId,
                        FullName = e.Student.FullName,
                        Email = e.Student.Email,
                        DateOfBirth = e.Student.DateOfBirth
                    } : null,
                    Course = null
                }).ToList() : null,
                Students = c.Enrollments != null && c.Enrollments.Any(e => e.Student != null) ? c.Enrollments.Where(e => e.Student != null).Select(e => new StudentBusinessModel
                {
                    StudentId = e.Student.StudentId,
                    FullName = e.Student.FullName,
                    Email = e.Student.Email,
                    DateOfBirth = e.Student.DateOfBirth
                }).ToList() : null
            };
        }

        public async Task<CourseBusinessModel> CreateCourseAsync(CourseBusinessModel model)
        {
            var course = new Repositories.Entities.Course
            {
                CourseName = model.CourseName,
                SemesterId = model.SemesterId
            };

            var created = await _repository.CreateCourseAsync(course);
            model.CourseId = created.CourseId;
            return model;
        }

        public async Task<bool> UpdateCourseAsync(int id, CourseBusinessModel model)
        {
            var course = await _repository.GetCourseByIdAsync(id);
            if (course == null) return false;

            course.CourseName = model.CourseName;
            course.SemesterId = model.SemesterId;

            await _repository.UpdateCourseAsync(course);
            return true;
        }

        public async Task<bool> DeleteCourseAsync(int id)
        {
            var course = await _repository.GetCourseByIdAsync(id);
            if (course == null) return false;

            await _repository.DeleteCourseAsync(id);
            return true;
        }
    }
}
