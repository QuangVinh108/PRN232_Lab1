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
    public class CourseRepository : ICourseRepository
    {
        private readonly ApplicationDbContext _context;

        public CourseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<Course> Items, int TotalCount)> GetAllCoursesAsync(string? search, string? sort, int page, int size, string? expand)
        {
            var query = _context.Courses.AsQueryable();
            
            if (!string.IsNullOrWhiteSpace(expand))
            {
                if (expand.Contains("student", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Include(c => c.Enrollments).ThenInclude(e => e.Student);
                }
                else if (expand.Contains("enrollment", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Include(c => c.Enrollments);
                }
                if (expand.Contains("semester", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Include(c => c.Semester);
                }
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(c => c.CourseName.Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(sort))
            {
                query = sort switch
                {
                    "courseName" => query.OrderBy(c => c.CourseName),
                    "-courseName" => query.OrderByDescending(c => c.CourseName),
                    _ => query.OrderBy(c => c.CourseId)
                };
            }

            int totalCount = await query.CountAsync();
            var items = await query.Skip((page - 1) * size).Take(size).ToListAsync();

            return (items, totalCount);
        }

        public async Task<Course> GetCourseByIdAsync(int id, string? expand = null)
        {
            var query = _context.Courses.AsQueryable();

            if (!string.IsNullOrWhiteSpace(expand))
            {
                if (expand.Contains("student", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Include(c => c.Enrollments).ThenInclude(e => e.Student);
                }
                else if (expand.Contains("enrollment", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Include(c => c.Enrollments);
                }
                if (expand.Contains("semester", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Include(c => c.Semester);
                }
            }

            return await query.FirstOrDefaultAsync(c => c.CourseId == id);
        }

        public async Task<Course> CreateCourseAsync(Course course)
        {
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
            return course;
        }

        public async Task UpdateCourseAsync(Course course)
        {
            _context.Courses.Update(course);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCourseAsync(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
            }
        }
    }
}
