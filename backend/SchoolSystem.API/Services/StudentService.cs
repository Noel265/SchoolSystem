using Microsoft.EntityFrameworkCore;
using SchoolSystem.API.Data;
using SchoolSystem.API.DTOs;
using SchoolSystem.API.Models;

namespace SchoolSystem.API.Services
{
    public class StudentService
    {
        private readonly AppDbContext _context;

        public StudentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<StudentResponseDTO>> GetAllAsync()
        {
            return await _context.Students
                .Include(s => s.Class)
                .Where(s => s.IsActive)
                .Select(s => new StudentResponseDTO
                {
                    Id = s.Id,
                    FullName = s.FullName,
                    AdmissionNumber = s.AdmissionNumber,
                    DateOfBirth = s.DateOfBirth,
                    Gender = s.Gender,
                    SchoolLevel = s.SchoolLevel,
                    ClassName = s.Class.Name,
                    EnrollmentDate = s.EnrollmentDate,
                    IsActive = s.IsActive
                })
                .ToListAsync();
        }

        public async Task<StudentResponseDTO?> GetByIdAsync(int id)
        {
            return await _context.Students
                .Include(s => s.Class)
                .Where(s => s.Id == id)
                .Select(s => new StudentResponseDTO
                {
                    Id = s.Id,
                    FullName = s.FullName,
                    AdmissionNumber = s.AdmissionNumber,
                    DateOfBirth = s.DateOfBirth,
                    Gender = s.Gender,
                    SchoolLevel = s.SchoolLevel,
                    ClassName = s.Class.Name,
                    EnrollmentDate = s.EnrollmentDate,
                    IsActive = s.IsActive
                })
                .FirstOrDefaultAsync();
        }

        public async Task<StudentResponseDTO?> CreateAsync(CreateStudentDTO dto)
        {
            if (await _context.Students.AnyAsync(s => s.AdmissionNumber == dto.AdmissionNumber))
                return null;

            var classExists = await _context.Classes.AnyAsync(c => c.Id == dto.ClassId);
            if (!classExists) return null;

            var student = new Student
            {
                FullName = dto.FullName,
                AdmissionNumber = dto.AdmissionNumber,
                DateOfBirth = dto.DateOfBirth,
                Gender = dto.Gender,
                SchoolLevel = dto.SchoolLevel,
                ClassId = dto.ClassId,
                EnrollmentDate = DateOnly.FromDateTime(DateTime.UtcNow)
            };

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(student.Id);
        }

        public async Task<StudentResponseDTO?> UpdateAsync(int id, UpdateStudentDTO dto)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null) return null;

            var classExists = await _context.Classes.AnyAsync(c => c.Id == dto.ClassId);
            if (!classExists) return null;

            student.FullName = dto.FullName;
            student.DateOfBirth = dto.DateOfBirth;
            student.Gender = dto.Gender;
            student.SchoolLevel = dto.SchoolLevel;
            student.ClassId = dto.ClassId;

            await _context.SaveChangesAsync();
            return await GetByIdAsync(student.Id);
        }

        public async Task<bool> DeactivateAsync(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null) return false;

            student.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<StudentResponseDTO>> GetByClassAsync(int classId)
        {
            return await _context.Students
                .Include(s => s.Class)
                .Where(s => s.ClassId == classId && s.IsActive)
                .Select(s => new StudentResponseDTO
                {
                    Id = s.Id,
                    FullName = s.FullName,
                    AdmissionNumber = s.AdmissionNumber,
                    DateOfBirth = s.DateOfBirth,
                    Gender = s.Gender,
                    SchoolLevel = s.SchoolLevel,
                    ClassName = s.Class.Name,
                    EnrollmentDate = s.EnrollmentDate,
                    IsActive = s.IsActive
                })
                .ToListAsync();
        }
    }
}