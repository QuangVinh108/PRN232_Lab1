using PRN232.Lab1.Services.BusinessModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PRN232.Lab1.Services.IService
{
    public interface ISemesterService
    {
        Task<(IEnumerable<SemesterBusinessModel> Items, int TotalCount)> GetAllSemestersAsync(string? search, string? sort, int page, int size, string? expand);
        Task<SemesterBusinessModel> GetSemesterByIdAsync(int id);
        Task<SemesterBusinessModel> CreateSemesterAsync(SemesterBusinessModel model);
        Task<bool> UpdateSemesterAsync(int id, SemesterBusinessModel model);
        Task<bool> DeleteSemesterAsync(int id);
    }
}
