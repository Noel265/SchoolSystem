using Microsoft.EntityFrameworkCore;
using SchoolSystem.API.Data;
using SchoolSystem.API.DTOs;
using SchoolSystem.API.Models;

namespace SchoolSystem.API.Services
{
    public class ClassService
    {
        private readonly AppDbContext _context;

        public ClassService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ClassResponseDTO>> GetAllAsync()
        {
            return await _context.Classes
                .Include(c => c.ClassTeacher)
                .Include(c => c.Students)
                .Select(c => new ClassResponseDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    SchoolLevel = c.SchoolLevel,
                    AcademicYear = c.AcademicYear,
                    ClassTeacherName = c.ClassTeacher != null ? c.ClassTeacher.FullName : null,
                    StudentCount = c.Students.Count(s => s.IsActive)
                })
                .ToListAsync();
        }

        public async Task<ClassResponseDTO?> GetByIdAsync(int id)
        {
            return await _context.Classes
                .Include(c => c.ClassTeacher)
                .Include(c => c.Students)
                .Where(c => c.Id == id)
                .Select(c => new ClassResponseDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    SchoolLevel = c.SchoolLevel,
                    AcademicYear = c.AcademicYear,
                    ClassTeacherName = c.ClassTeacher != null ? c.ClassTeacher.FullName : null,
                    StudentCount = c.Students.Count(s => s.IsActive)
                })
                .FirstOrDefaultAsync();
        }

        public async Task<ClassResponseDTO?> CreateAsync(CreateClassDTO dto)
        {
           // Check teacher exists if provided
            if (dto.ClassTeacherId.HasValue)
            {
                var teacherExists = await _context.Teachers
                    .AnyAsync(t => t.Id == dto.ClassTeacherId.Value);
                if (!teacherExists) return null;
            }

            var newClass = new Class
            {
                Name = dto.Name,
                SchoolLevel = dto.SchoolLevel,
                AcademicYear = dto.AcademicYear,
                ClassTeacherId = dto.ClassTeacherId
            };

            _context.Classes.Add(newClass);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(newClass.Id);
        }

        public async Task<ClassResponseDTO?> UpdateAsync(int id, UpdateClassDTO dto)
        {
            var existing = await _context.Classes.FindAsync(id);
            if (existing == null) return null;

            existing.Name = dto.Name;
            existing.SchoolLevel = dto.SchoolLevel;
            existing.AcademicYear = dto.AcademicYear;
            existing.ClassTeacherId = dto.ClassTeacherId;

            await _context.SaveChangesAsync();
            return await GetByIdAsync(existing.Id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.Classes.FindAsync(id);
            if (existing == null) return false;

            var hasStudents = await _context.Students
                .AnyAsync(s => s.ClassId == id && s.IsActive);
            if (hasStudents) return false; // Prevent deletion if active students are enrolled

            _context.Classes.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}