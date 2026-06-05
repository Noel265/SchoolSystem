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

        public async Task<List<SubjectResponseDTO>> GetAllAsync()
        {
            return await _context.Subjects
                .OrderBy(s => s.SchoolLevel)
                .ThenBy(s => s.Name)
                .Select(s => new SubjectResponseDTO
                {
                    Id = s.Id,
                    Name = s.Name,
                    Code = s.Code,
                    SchoolLevel = s.SchoolLevel,
                    IsActive = s.IsActive
                })
                .ToListAsync();
        }

        public async Task<SubjectResponseDTO?> CreateAsync(CreateSubjectDTO dto)
        {
            if (await _context.Subjects.AnyAsync(s => s.Code == dto.Code))
                return null;

            var subject = new Subject
            {
                Name = dto.Name,
                Code = dto.Code,
                SchoolLevel = dto.SchoolLevel,
                IsActive = true
            };

            _context.Subjects.Add(subject);
            await _context.SaveChangesAsync();

            return ToSubjectResponse(subject);
        }

        public async Task<SubjectResponseDTO?> UpdateAsync(int id, UpdateSubjectDTO dto)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null) return null;

            if (await _context.Subjects.AnyAsync(s => s.Id != id && s.Code == dto.Code))
                return null;

            subject.Name = dto.Name;
            subject.Code = dto.Code;
            subject.SchoolLevel = dto.SchoolLevel;
            subject.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();
            return ToSubjectResponse(subject);
        }

        public async Task<List<ClassSubjectResponseDTO>> GetAssignmentsAsync(int? classId = null, int? teacherId = null)
        {
            var query = _context.ClassSubjects
                .Include(cs => cs.Class)
                .Include(cs => cs.Subject)
                .Include(cs => cs.Teacher)
                .AsQueryable();

            if (classId.HasValue)
                query = query.Where(cs => cs.ClassId == classId.Value);

            if (teacherId.HasValue)
                query = query.Where(cs => cs.TeacherId == teacherId.Value);

            return await query
                .OrderBy(cs => cs.Class.Name)
                .ThenBy(cs => cs.Subject.Name)
                .Select(cs => new ClassSubjectResponseDTO
                {
                    Id = cs.Id,
                    ClassId = cs.ClassId,
                    ClassName = cs.Class.Name,
                    SchoolLevel = cs.Class.SchoolLevel,
                    SubjectId = cs.SubjectId,
                    SubjectName = cs.Subject.Name,
                    SubjectCode = cs.Subject.Code,
                    TeacherId = cs.TeacherId,
                    TeacherName = cs.Teacher.FullName,
                    IsActive = cs.IsActive
                })
                .ToListAsync();
        }

        public async Task<ClassSubjectResponseDTO?> AssignAsync(AssignClassSubjectDTO dto)
        {
            var schoolClass = await _context.Classes.FindAsync(dto.ClassId);
            var subject = await _context.Subjects.FindAsync(dto.SubjectId);
            var teacher = await _context.Teachers.FindAsync(dto.TeacherId);

            if (schoolClass == null || subject == null || teacher == null)
                return null;

            if (schoolClass.SchoolLevel != subject.SchoolLevel)
                return null;

            var existing = await _context.ClassSubjects
                .FirstOrDefaultAsync(cs => cs.ClassId == dto.ClassId && cs.SubjectId == dto.SubjectId);

            if (existing != null)
            {
                existing.TeacherId = dto.TeacherId;
                existing.IsActive = true;
            }
            else
            {
                _context.ClassSubjects.Add(new ClassSubject
                {
                    ClassId = dto.ClassId,
                    SubjectId = dto.SubjectId,
                    TeacherId = dto.TeacherId,
                    IsActive = true
                });
            }

            await _context.SaveChangesAsync();

            return (await GetAssignmentsAsync(dto.ClassId))
                .FirstOrDefault(cs => cs.SubjectId == dto.SubjectId);
        }

        public async Task<bool> DeactivateAssignmentAsync(int id)
        {
            var assignment = await _context.ClassSubjects.FindAsync(id);
            if (assignment == null) return false;

            assignment.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        private static SubjectResponseDTO ToSubjectResponse(Subject subject)
        {
            return new SubjectResponseDTO
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
