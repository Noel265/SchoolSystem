using Microsoft.EntityFrameworkCore;
using SchoolSystem.API.Data;
using SchoolSystem.API.DTOs;
using SchoolSystem.API.Models;

namespace SchoolSystem.API.Services
{
    public class TeacherService
    {
        private readonly AppDbContext _context;

        public TeacherService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<TeacherResponseDTO>> GetAllAsync()
        {
            return await _context.Teachers
                .Include(t => t.User)
                .Where(t => t.IsActive)
                .Select(t => new TeacherResponseDTO
                {
                    Id = t.Id,
                    FullName = t.FullName,
                    EmployeeNumber = t.EmployeeNumber,
                    PhoneNumber = t.PhoneNumber,
                    Specialization = t.Specialization,
                    HireDate = t.HireDate,
                    Email = t.User.Email,
                    IsActive = t.IsActive
                })
                .ToListAsync();
        }

        public async Task<TeacherResponseDTO?> GetByIdAsync(int id)
        {
            return await _context.Teachers
                .Include(t => t.User)
                .Where(t => t.Id == id)
                .Select(t => new TeacherResponseDTO
                {
                    Id = t.Id,
                    FullName = t.FullName,
                    EmployeeNumber = t.EmployeeNumber,
                    PhoneNumber = t.PhoneNumber,
                    Specialization = t.Specialization,
                    HireDate = t.HireDate,
                    Email = t.User.Email,
                    IsActive = t.IsActive
                })
                .FirstOrDefaultAsync();
        }

        public async Task<TeacherResponseDTO?> CreateAsync(CreateTeacherDTO dto)
        {
            // Check email is unique
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                return null;

            // Check employee number is unique
            if (await _context.Teachers.AnyAsync(t => t.EmployeeNumber == dto.EmployeeNumber))
                return null;

            // Create user account for teacher
            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = "Teacher"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Create teacher record
            var teacher = new Teacher
            {
                FullName = dto.FullName,
                EmployeeNumber = dto.EmployeeNumber,
                PhoneNumber = dto.PhoneNumber,
                Specialization = dto.Specialization,
                HireDate = dto.HireDate,
                UserId = user.Id
            };

            _context.Teachers.Add(teacher);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(teacher.Id);
        }

        public async Task<TeacherResponseDTO?> UpdateAsync(int id, UpdateTeacherDTO dto)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null) return null;

            teacher.FullName = dto.FullName;
            teacher.PhoneNumber = dto.PhoneNumber;
            teacher.Specialization = dto.Specialization;

            await _context.SaveChangesAsync();
            return await GetByIdAsync(teacher.Id);
        }

        public async Task<bool> DeactivateAsync(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null) return false;

            teacher.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}