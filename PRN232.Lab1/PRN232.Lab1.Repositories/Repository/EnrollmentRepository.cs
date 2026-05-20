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
    public class EnrollmentRepository : IEnrollmentRepository
    {
        private readonly ApplicationDbContext _context;

        public EnrollmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<Enrollment> Items, int TotalCount)> GetAllEnrollmentsAsync(string? search, string? sort, int page, int size, string? expand)
        {
            var query = _context.Enrollments.AsQueryable();
            
            if (!string.IsNullOrWhiteSpace(expand))
            {
                if (expand.Contains("student", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Include(e => e.Student);
                }
                if (expand.Contains("course", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Include(e => e.Course);
                }
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(e => e.Status.Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(sort))
            {
                query = sort switch
                {
                    "enrollDate" => query.OrderBy(e => e.EnrollDate),
                    "-enrollDate" => query.OrderByDescending(e => e.EnrollDate),
                    _ => query.OrderBy(e => e.EnrollmentId)
                };
            }

            int totalCount = await query.CountAsync();
            var items = await query.Skip((page - 1) * size).Take(size).ToListAsync();

            return (items, totalCount);
        }

        public async Task<Enrollment> GetEnrollmentByIdAsync(int id)
        {
            return await _context.Enrollments.FirstOrDefaultAsync(e => e.EnrollmentId == id);
        }

        public async Task<Enrollment> CreateEnrollmentAsync(Enrollment enrollment)
        {
            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();
            return enrollment;
        }

        public async Task UpdateEnrollmentAsync(Enrollment enrollment)
        {
            _context.Enrollments.Update(enrollment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteEnrollmentAsync(int id)
        {
            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment != null)
            {
                _context.Enrollments.Remove(enrollment);
                await _context.SaveChangesAsync();
            }
        }
    }
}
