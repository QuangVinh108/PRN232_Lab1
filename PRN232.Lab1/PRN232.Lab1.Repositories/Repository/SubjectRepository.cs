using Microsoft.EntityFrameworkCore;
using PRN232.Lab1.Repositories.Data;
using PRN232.Lab1.Repositories.Entities;
using PRN232.Lab1.Repositories.IRepository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PRN232.Lab1.Repositories.Repository
{
    public class SubjectRepository : ISubjectRepository
    {
        private readonly ApplicationDbContext _context;

        public SubjectRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<Subject> Items, int TotalCount)> GetAllSubjectsAsync(string? search, string? sort, int page, int size, string? expand)
        {
            var query = _context.Subjects.AsQueryable();
            
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(s => s.SubjectName.Contains(search) || s.SubjectCode.Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(sort))
            {
                query = sort switch
                {
                    "subjectName" => query.OrderBy(s => s.SubjectName),
                    "-subjectName" => query.OrderByDescending(s => s.SubjectName),
                    "subjectCode" => query.OrderBy(s => s.SubjectCode),
                    "-subjectCode" => query.OrderByDescending(s => s.SubjectCode),
                    _ => query.OrderBy(s => s.SubjectId)
                };
            }

            int totalCount = await query.CountAsync();
            var items = await query.Skip((page - 1) * size).Take(size).ToListAsync();

            return (items, totalCount);
        }

        public async Task<Subject> GetSubjectByIdAsync(int id)
        {
            return await _context.Subjects.FirstOrDefaultAsync(s => s.SubjectId == id);
        }

        public async Task<Subject> CreateSubjectAsync(Subject subject)
        {
            _context.Subjects.Add(subject);
            await _context.SaveChangesAsync();
            return subject;
        }

        public async Task UpdateSubjectAsync(Subject subject)
        {
            _context.Subjects.Update(subject);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSubjectAsync(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject != null)
            {
                _context.Subjects.Remove(subject);
                await _context.SaveChangesAsync();
            }
        }
    }
}
