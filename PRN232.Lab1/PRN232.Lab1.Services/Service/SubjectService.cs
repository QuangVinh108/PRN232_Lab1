using PRN232.Lab1.Repositories.IRepository;
using PRN232.Lab1.Services.BusinessModels;
using PRN232.Lab1.Services.IService;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PRN232.Lab1.Services.Service
{
    public class SubjectService : ISubjectService
    {
        private readonly ISubjectRepository _repository;

        public SubjectService(ISubjectRepository repository)
        {
            _repository = repository;
        }

        public async Task<(IEnumerable<SubjectBusinessModel> Items, int TotalCount)> GetAllSubjectsAsync(string? search, string? sort, int page, int size, string? expand)
        {
            var result = await _repository.GetAllSubjectsAsync(search, sort, page, size, expand);

            var mappedItems = result.Items.Select(s => new SubjectBusinessModel
            {
                SubjectId = s.SubjectId,
                SubjectCode = s.SubjectCode,
                SubjectName = s.SubjectName,
                Credit = s.Credit
            });

            return (mappedItems, result.TotalCount);
        }

        public async Task<SubjectBusinessModel> GetSubjectByIdAsync(int id)
        {
            var s = await _repository.GetSubjectByIdAsync(id);
            if (s == null) return null;

            return new SubjectBusinessModel
            {
                SubjectId = s.SubjectId,
                SubjectCode = s.SubjectCode,
                SubjectName = s.SubjectName,
                Credit = s.Credit
            };
        }

        public async Task<SubjectBusinessModel> CreateSubjectAsync(SubjectBusinessModel model)
        {
            var subject = new Repositories.Entities.Subject
            {
                SubjectCode = model.SubjectCode,
                SubjectName = model.SubjectName,
                Credit = model.Credit
            };

            var created = await _repository.CreateSubjectAsync(subject);
            model.SubjectId = created.SubjectId;
            return model;
        }

        public async Task<bool> UpdateSubjectAsync(int id, SubjectBusinessModel model)
        {
            var subject = await _repository.GetSubjectByIdAsync(id);
            if (subject == null) return false;

            subject.SubjectCode = model.SubjectCode;
            subject.SubjectName = model.SubjectName;
            subject.Credit = model.Credit;

            await _repository.UpdateSubjectAsync(subject);
            return true;
        }

        public async Task<bool> DeleteSubjectAsync(int id)
        {
            var subject = await _repository.GetSubjectByIdAsync(id);
            if (subject == null) return false;

            await _repository.DeleteSubjectAsync(id);
            return true;
        }
    }
}
