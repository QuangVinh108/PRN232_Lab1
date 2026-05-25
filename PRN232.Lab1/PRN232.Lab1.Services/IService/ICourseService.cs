using PRN232.Lab1.Services.BusinessModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PRN232.Lab1.Services.IService
{
    public interface ICourseService
    {
        Task<(IEnumerable<CourseBusinessModel> Items, int TotalCount)> GetAllCoursesAsync(string? search, string? sort, int page, int size, string? expand);
        Task<CourseBusinessModel> GetCourseByIdAsync(int id, string? expand = null);
        Task<CourseBusinessModel> CreateCourseAsync(CourseBusinessModel model);
        Task<bool> UpdateCourseAsync(int id, CourseBusinessModel model);
        Task<bool> DeleteCourseAsync(int id);
    }
}
