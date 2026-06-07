using Microsoft.EntityFrameworkCore;
using SchoolSystem.API.Data;
using SchoolSystem.API.DTOs;
using SchoolSystem.API.Models;

namespace SchoolSystem.API.Services
{
    public class ParentService
    {
        private readonly AppDbContext _context;

        public ParentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ParentResponseDTO>> GetAllAsync()
        {
            var parents = await _context.Parents
                .Include(p => p.User)
                .Include(p => p.StudentParents)
                    .ThenInclude(sp => sp.Student)
                        .ThenInclude(s => s.Class)
                .ToListAsync();

            return parents.Select(p =>
            {
                var sp = p.StudentParents.FirstOrDefault();
                return new ParentResponseDTO
                {
                    Id = p.Id,
                    FullName = p.FullName,
                    PhoneNumber = p.PhoneNumber,
                    Address = p.Address,
                    Email = p.User.Email,
                    Relationship = sp?.Relationship ?? "",
                    StudentId = sp?.StudentId ?? 0,
                    StudentName = sp?.Student.FullName ?? "",
                    AdmissionNumber = sp?.Student.AdmissionNumber ?? "",
                    IsActive = p.User.IsActive
                };
            }).ToList();
        }

        public async Task<ParentResponseDTO?> GetByIdAsync(int id)
        {
            var p = await _context.Parents
                .Include(p => p.User)
                .Include(p => p.StudentParents)
                    .ThenInclude(sp => sp.Student)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (p == null) return null;

            var sp = p.StudentParents.FirstOrDefault();
            return new ParentResponseDTO
            {
                Id = p.Id,
                FullName = p.FullName,
                PhoneNumber = p.PhoneNumber,
                Address = p.Address,
                Email = p.User.Email,
                Relationship = sp?.Relationship ?? "",
                StudentId = sp?.StudentId ?? 0,
                StudentName = sp?.Student.FullName ?? "",
                AdmissionNumber = sp?.Student.AdmissionNumber ?? "",
                IsActive = p.User.IsActive
            };
        }

        public async Task<ParentStudentDTO?> GetStudentByParentUserIdAsync(int userId)
        {
            var parent = await _context.Parents
                .Include(p => p.StudentParents)
                    .ThenInclude(sp => sp.Student)
                        .ThenInclude(s => s.Class)
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (parent == null) return null;

            var sp = parent.StudentParents.FirstOrDefault();
            if (sp == null) return null;

            return new ParentStudentDTO
            {
                StudentId = sp.StudentId,
                StudentName = sp.Student.FullName,
                AdmissionNumber = sp.Student.AdmissionNumber,
                ClassName = sp.Student.Class.Name,
                Relationship = sp.Relationship
            };
        }

        public async Task<ParentResponseDTO?> CreateAsync(CreateParentDTO dto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                return null;

            var student = await _context.Students.FindAsync(dto.StudentId);
            if (student == null) return null;

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = "Parent"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var parent = new Parent
            {
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address,
                UserId = user.Id
            };

            _context.Parents.Add(parent);
            await _context.SaveChangesAsync();

            _context.StudentParents.Add(new StudentParent
            {
                StudentId = dto.StudentId,
                ParentId = parent.Id,
                Relationship = dto.Relationship
            });

            await _context.SaveChangesAsync();
            return await GetByIdAsync(parent.Id);
        }

        public async Task<bool> DeactivateAsync(int id)
        {
            var parent = await _context.Parents
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (parent == null) return false;

            parent.User.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}