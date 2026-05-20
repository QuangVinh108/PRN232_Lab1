using PRN232.Lab1.Repositories.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PRN232.Lab1.Repositories.IRepository
{
    public interface ISemesterRepository
    {
        Task<(IEnumerable<Semester> Items, int TotalCount)> GetAllSemestersAsync(string? search, string? sort, int page, int size, string? expand);
        Task<Semester> GetSemesterByIdAsync(int id);
        Task<Semester> CreateSemesterAsync(Semester semester);
        Task UpdateSemesterAsync(Semester semester);
        Task DeleteSemesterAsync(int id);
    }
}
