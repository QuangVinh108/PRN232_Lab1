using PRN232.Lab1.Repositories.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PRN232.Lab1.Repositories.IRepository
{
    public interface ISubjectRepository
    {
        Task<(IEnumerable<Subject> Items, int TotalCount)> GetAllSubjectsAsync(string? search, string? sort, int page, int size, string? expand);
        Task<Subject> GetSubjectByIdAsync(int id);
        Task<Subject> CreateSubjectAsync(Subject subject);
        Task UpdateSubjectAsync(Subject subject);
        Task DeleteSubjectAsync(int id);
    }
}
