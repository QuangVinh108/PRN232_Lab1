using PRN232.Lab1.Repositories.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PRN232.Lab1.Repositories.IRepository
{
    public interface IEnrollmentRepository
    {
        Task<(IEnumerable<Enrollment> Items, int TotalCount)> GetAllEnrollmentsAsync(string? search, string? sort, int page, int size, string? expand);
        Task<Enrollment> GetEnrollmentByIdAsync(int id, string? expand = null);
        Task<Enrollment> CreateEnrollmentAsync(Enrollment enrollment);
        Task UpdateEnrollmentAsync(Enrollment enrollment);
        Task DeleteEnrollmentAsync(int id);
    }
}
