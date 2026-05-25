using PRN232.Lab1.Repositories.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PRN232.Lab1.Repositories.IRepository
{
    public interface ICourseRepository
    {
        Task<(IEnumerable<Course> Items, int TotalCount)> GetAllCoursesAsync(string? search, string? sort, int page, int size, string? expand);
        Task<Course> GetCourseByIdAsync(int id, string? expand = null);
        Task<Course> CreateCourseAsync(Course course);
        Task UpdateCourseAsync(Course course);
        Task DeleteCourseAsync(int id);
    }
}
