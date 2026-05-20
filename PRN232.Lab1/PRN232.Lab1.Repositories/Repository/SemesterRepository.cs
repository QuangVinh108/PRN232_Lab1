using Microsoft.EntityFrameworkCore;
using PRN232.Lab1.Repositories.Data;
using PRN232.Lab1.Repositories.Entities;
using PRN232.Lab1.Repositories.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PRN232.Lab1.Repositories.Repository
{
    public class SemesterRepository : ISemesterRepository
    {
        private readonly ApplicationDbContext _context;

        public SemesterRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<Semester> Items, int TotalCount)> GetAllSemestersAsync(string? search, string? sort, int page, int size, string? expand)
        {
            var query = _context.Semesters.AsQueryable();
            
            if (!string.IsNullOrWhiteSpace(expand) && expand.Contains("course", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Include(s => s.Courses);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(s => s.SemesterName.Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(sort))
            {
                query = sort switch
                {
                    "semesterName" => query.OrderBy(s => s.SemesterName),
                    "-semesterName" => query.OrderByDescending(s => s.SemesterName),
                    "startDate" => query.OrderBy(s => s.StartDate),
                    "-startDate" => query.OrderByDescending(s => s.StartDate),
                    _ => query.OrderBy(s => s.SemesterId)
                };
            }

            int totalCount = await query.CountAsync();
            var items = await query.Skip((page - 1) * size).Take(size).ToListAsync();

            return (items, totalCount);
        }

        public async Task<Semester> GetSemesterByIdAsync(int id)
        {
            return await _context.Semesters.FirstOrDefaultAsync(s => s.SemesterId == id);
        }

        public async Task<Semester> CreateSemesterAsync(Semester semester)
        {
            _context.Semesters.Add(semester);
            await _context.SaveChangesAsync();
            return semester;
        }

        public async Task UpdateSemesterAsync(Semester semester)
        {
            _context.Semesters.Update(semester);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSemesterAsync(int id)
        {
            var semester = await _context.Semesters.FindAsync(id);
            if (semester != null)
            {
                _context.Semesters.Remove(semester);
                await _context.SaveChangesAsync();
            }
        }
    }
}
