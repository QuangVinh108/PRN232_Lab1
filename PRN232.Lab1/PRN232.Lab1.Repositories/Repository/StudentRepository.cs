using Microsoft.EntityFrameworkCore;
using PRN232.Lab1.Repositories.Data;
using PRN232.Lab1.Repositories.Entities;
using PRN232.Lab1.Repositories.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.Lab1.Repositories.Repository
{
    public class StudentRepository : IStudentRepository
    {
        private readonly ApplicationDbContext _context;

        public StudentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<Student> Items, int TotalCount)> GetAllStudentsAsync(string? search, string? sort, int page, int size, string? expand)
        {
            var query = _context.Students.AsQueryable();
            if (!string.IsNullOrWhiteSpace(expand) && expand.Contains("enrollment", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Include(s => s.Enrollments);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(s => s.FullName.Contains(search) || s.Email.Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(sort))
            {
                query = sort switch
                {
                    "fullName" => query.OrderBy(s => s.FullName),
                    "-fullName" => query.OrderByDescending(s => s.FullName),
                    "dateOfBirth" => query.OrderBy(s => s.DateOfBirth),
                    "-dateOfBirth" => query.OrderByDescending(s => s.DateOfBirth),
                    _ => query.OrderBy(s => s.StudentId) // Mặc định
                };
            }

            int totalCount = await query.CountAsync();

            var items = await query.Skip((page - 1) * size).Take(size).ToListAsync();

            return (items, totalCount);
        }

        public async Task<Student> GetStudentByIdAsync(int id)
        {
            return await _context.Students.FirstOrDefaultAsync(s => s.StudentId == id);
        }

        public async Task<Student> CreateStudentAsync(Student student)
        {
            _context.Students.Add(student);
            await _context.SaveChangesAsync();
            return student;
        }

        public async Task UpdateStudentAsync(Student student)
        {
            _context.Students.Update(student);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteStudentAsync(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
            }
        }
    }
}
