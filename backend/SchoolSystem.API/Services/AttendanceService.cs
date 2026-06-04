using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SchoolSystem.API.Data;
using SchoolSystem.API.DTOs;
using SchoolSystem.API.Models;

namespace SchoolSystem.API.Services
{
    public class AttendanceService
    {
        private readonly AppDbContext _context;

        public AttendanceService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<AttendanceResponseDTO>> GetByClassAndDateAsync(int classId, DateOnly date)
        {
            return await _context.Attendances
                .Include(a => a.Student)
                .Include(a => a.Teacher)
                .Where(a => a.Student.ClassId == classId && a.Date == date)
                .Select(a => new AttendanceResponseDTO
                {
                    Id = a.Id,
                    StudentName = a.Student.FullName,
                    AdmissionNumber = a.Student.AdmissionNumber,
                    Date = a.Date,
                    Status = a.Status,
                    Remarks = a.Remarks,
                    TeacherName = a.Teacher.FullName
                })
                .ToListAsync();
        }

        public async Task<List<AttendanceResponseDTO>> GetByStudentAsync(int studentId)
        {
            return await _context.Attendances
                .Include(a => a.Student)
                .Include(a => a.Teacher)
                .Where(a => a.StudentId == studentId)
                .OrderByDescending(a => a.Date)
                .Select(a => new AttendanceResponseDTO
                {
                    Id = a.Id,
                    StudentName = a.Student.FullName,
                    AdmissionNumber = a.Student.AdmissionNumber,
                    Date = a.Date,
                    Status = a.Status,
                    Remarks = a.Remarks,
                    TeacherName = a.Teacher.FullName
                })
                .ToListAsync();
        }

        public async Task<bool> BulkMarkAsync(BulkAttendanceDTO dto, int teacherId)
        {
            // Check teacher exists
            var teacher = await _context.Teachers.FindAsync(teacherId);
            if (teacher == null) return false;

            foreach (var item in dto.Attendances)
            {
                // Check if attendance already marked for this student on this date
                var existing = await _context.Attendances
                    .FirstOrDefaultAsync(a => a.StudentId == item.StudentId && a.Date == dto.Date);

                if (existing != null)
                {
                    // Update existing
                    existing.Status = item.Status;
                    existing.Remarks = item.Remarks;
                }
                else
                {
                    // Create new
                    _context.Attendances.Add(new Attendance
                    {
                        StudentId = item.StudentId,
                        Date = dto.Date,
                        Status = item.Status,
                        Remarks = item.Remarks,
                        TeacherId = teacherId
                    });
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<AttendanceSummaryDTO?> GetStudentSummaryAsync(int studentId)
        {
            var student = await _context.Students.FindAsync(studentId);
            if (student == null) return null;

            var records = await _context.Attendances
                .Where(a => a.StudentId == studentId)
                .ToListAsync();

            return new AttendanceSummaryDTO
            {
                StudentId = studentId,
                StudentName = student.FullName,
                TotalDays = records.Count,
                PresentDays = records.Count(a => a.Status == "Present"),
                AbsentDays = records.Count(a => a.Status == "Absent"),
                LateDays = records.Count(a => a.Status == "Late"),
                AttendancePercentage = records.Count == 0 ? 0 :
                    Math.Round((double)records.Count(a => a.Status == "Present") / records.Count * 100, 1)
            };
        }
    }
}