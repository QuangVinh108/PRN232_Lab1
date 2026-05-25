using PRN232.Lab1.Services.BusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.Lab1.Services.IService
{
    public interface IStudentService
    {
        Task<(IEnumerable<StudentBusinessModel> Items, int TotalCount)> GetAllStudentsAsync(string? search, string? sort, int page, int size, string? expand);
        Task<StudentBusinessModel> GetStudentByIdAsync(int id, string? expand = null);
        Task<StudentBusinessModel> CreateStudentAsync(StudentBusinessModel model);
        Task<bool> UpdateStudentAsync(int id, StudentBusinessModel model);
        Task<bool> DeleteStudentAsync(int id);
    }
}
