using Microsoft.EntityFrameworkCore;
using SchoolSystem.API.Data;
using SchoolSystem.API.DTOs;
using SchoolSystem.API.Models;

namespace SchoolSystem.API.Services
{
    public class SubjectService
    {
        private readonly AppDbContext _context;

        public SubjectService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<SubjectDTO?> CreateSubjectAsync(CreateSubjectDTO dto)
        {
            var subject = new Subject
            {
                Name = dto.Name,
                Code = dto.Code,
                SchoolLevel = dto.SchoolLevel,
                IsActive = true
            };

            _context.Subjects.Add(subject);
            await _context.SaveChangesAsync();

            return MapToDTO(subject);
        }

        public async Task<SubjectDTO?> GetSubjectAsync(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            return subject == null ? null : MapToDTO(subject);
        }

        public async Task<List<SubjectDTO>> GetSubjectsAsync(string? schoolLevel = null)
        {
            var query = _context.Subjects.AsQueryable();

            if (!string.IsNullOrEmpty(schoolLevel))
                query = query.Where(s => s.SchoolLevel == schoolLevel);

            var subjects = await query.ToListAsync();
            return subjects.Select(MapToDTO).ToList();
        }

        public async Task<SubjectDTO?> UpdateSubjectAsync(int id, UpdateSubjectDTO dto)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null) return null;

            subject.Name = dto.Name;
            subject.Code = dto.Code;
            subject.SchoolLevel = dto.SchoolLevel;
            subject.IsActive = dto.IsActive;

            _context.Subjects.Update(subject);
            await _context.SaveChangesAsync();

            return MapToDTO(subject);
        }

        public async Task<bool> DeleteSubjectAsync(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null) return false;

            _context.Subjects.Remove(subject);
            await _context.SaveChangesAsync();
            return true;
        }

        private SubjectDTO MapToDTO(Subject subject)
        {
            return new SubjectDTO
            {
                Id = subject.Id,
                Name = subject.Name,
                Code = subject.Code,
                SchoolLevel = subject.SchoolLevel,
                IsActive = subject.IsActive
            };
        }
    }
}
