using PRN232.Lab1.Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.Lab1.Repositories.IRepository
{
    public interface IStudentRepository
    {
        // Trả về một Tuple chứa danh sách Entity và tổng số dòng
        Task<(IEnumerable<Student> Items, int TotalCount)> GetAllStudentsAsync(string? search, string? sort, int page, int size, string? expand);
        Task<Student> GetStudentByIdAsync(int id, string? expand = null);
        Task<Student> CreateStudentAsync(Student student);
        Task UpdateStudentAsync(Student student);
        Task DeleteStudentAsync(int id);
    }
}
