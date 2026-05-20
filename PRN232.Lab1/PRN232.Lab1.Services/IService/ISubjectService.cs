using PRN232.Lab1.Services.BusinessModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PRN232.Lab1.Services.IService
{
    public interface ISubjectService
    {
        Task<(IEnumerable<SubjectBusinessModel> Items, int TotalCount)> GetAllSubjectsAsync(string? search, string? sort, int page, int size, string? expand);
        Task<SubjectBusinessModel> GetSubjectByIdAsync(int id);
        Task<SubjectBusinessModel> CreateSubjectAsync(SubjectBusinessModel model);
        Task<bool> UpdateSubjectAsync(int id, SubjectBusinessModel model);
        Task<bool> DeleteSubjectAsync(int id);
    }
}
