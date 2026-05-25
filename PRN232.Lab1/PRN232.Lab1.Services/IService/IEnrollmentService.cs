using PRN232.Lab1.Services.BusinessModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PRN232.Lab1.Services.IService
{
    public interface IEnrollmentService
    {
        Task<(IEnumerable<EnrollmentBusinessModel> Items, int TotalCount)> GetAllEnrollmentsAsync(string? search, string? sort, int page, int size, string? expand);
        Task<EnrollmentBusinessModel> GetEnrollmentByIdAsync(int id, string? expand = null);
        Task<EnrollmentBusinessModel> CreateEnrollmentAsync(EnrollmentBusinessModel model);
        Task<bool> UpdateEnrollmentAsync(int id, EnrollmentBusinessModel model);
        Task<bool> DeleteEnrollmentAsync(int id);
    }
}
