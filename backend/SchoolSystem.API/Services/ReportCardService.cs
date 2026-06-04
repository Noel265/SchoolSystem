using Microsoft.EntityFrameworkCore;
using SchoolSystem.API.Data;
using SchoolSystem.API.DTOs;
using SchoolSystem.API.Models;

namespace SchoolSystem.API.Services
{
    public class ReportCardService
    {
        private readonly AppDbContext _context;
        private const decimal PASS_THRESHOLD = 50m;

        public ReportCardService(AppDbContext context)
        {
            _context = context;
        }

        // Generate report card for a student
        public async Task<ReportCardDTO?> GenerateReportCardAsync(CreateReportCardDTO dto, int teacherId)
        {
            var student = await _context.Students
                .Include(s => s.Class)
                .ThenInclude(c => c.ClassTeacher)
                .FirstOrDefaultAsync(s => s.Id == dto.StudentId);

            if (student == null) return null;

            var grades = await _context.Grades
                .Where(g => g.StudentId == dto.StudentId && g.Term == dto.Term && g.AcademicYear == dto.AcademicYear)
                .Include(g => g.Subject)
                .ToListAsync();

            if (!grades.Any()) return null;

            var reportCard = new ReportCard
            {
                StudentId = dto.StudentId,
                ClassId = student.ClassId,
                TeacherId = teacherId,
                Term = dto.Term,
                AcademicYear = dto.AcademicYear,
                Comments = dto.Comments,
                GeneratedDate = DateTime.UtcNow
            };

            // Calculate overall GPA
            var overallGPA = grades.Average(g => g.Marks);
            reportCard.OverallGPA = overallGPA;
            reportCard.Status = overallGPA >= PASS_THRESHOLD ? "Pass" : "Fail";

            // Calculate class position
            var classGrades = await _context.Grades
                .Where(g => g.Student.ClassId == student.ClassId && g.Term == dto.Term && g.AcademicYear == dto.AcademicYear)
                .Include(g => g.Student)
                .ToListAsync();

            var studentAverages = classGrades
                .GroupBy(g => g.StudentId)
                .Select(g => new { StudentId = g.Key, Average = g.Average(x => x.Marks) })
                .OrderByDescending(x => x.Average)
                .ToList();

            reportCard.ClassPosition = studentAverages.FindIndex(x => x.StudentId == dto.StudentId) + 1;

            // Add subjects to report card
            var reportCardSubjects = new List<ReportCardSubject>();
            foreach (var grade in grades)
            {
                var classAverage = classGrades
                    .Where(g => g.SubjectId == grade.SubjectId)
                    .Average(g => g.Marks);

                reportCardSubjects.Add(new ReportCardSubject
                {
                    SubjectId = grade.SubjectId,
                    Marks = grade.Marks,
                    LetterGrade = grade.LetterGrade,
                    ClassAverage = classAverage
                });
            }

            reportCard.ReportCardSubjects = reportCardSubjects;

            _context.ReportCards.Add(reportCard);
            await _context.SaveChangesAsync();

            return MapToDTO(reportCard, student, student.Class.ClassTeacher);
        }

        // Get report card by ID
        public async Task<ReportCardDTO?> GetReportCardAsync(int reportCardId)
        {
            var reportCard = await _context.ReportCards
                .Include(r => r.Student)
                .Include(r => r.Class)
                .Include(r => r.Teacher)
                .Include(r => r.ReportCardSubjects)
                .ThenInclude(rs => rs.Subject)
                .FirstOrDefaultAsync(r => r.Id == reportCardId);

            if (reportCard == null) return null;

            return MapToDTO(reportCard, reportCard.Student, reportCard.Teacher);
        }

        // Get all report cards for a student
        public async Task<List<ReportCardDTO>> GetStudentReportCardsAsync(int studentId)
        {
            var reportCards = await _context.ReportCards
                .Where(r => r.StudentId == studentId)
                .Include(r => r.Student)
                .Include(r => r.Teacher)
                .Include(r => r.ReportCardSubjects)
                .ThenInclude(rs => rs.Subject)
                .ToListAsync();

            return reportCards.Select(r => MapToDTO(r, r.Student, r.Teacher)).ToList();
        }

        // Get all report cards for a class in a term
        public async Task<List<ReportCardDTO>> GetClassReportCardsAsync(int classId, string term, string academicYear)
        {
            var reportCards = await _context.ReportCards
                .Where(r => r.ClassId == classId && r.Term == term && r.AcademicYear == academicYear)
                .Include(r => r.Student)
                .Include(r => r.Teacher)
                .Include(r => r.ReportCardSubjects)
                .ThenInclude(rs => rs.Subject)
                .ToListAsync();

            return reportCards.Select(r => MapToDTO(r, r.Student, r.Teacher)).ToList();
        }

        // Update report card comments
        public async Task<ReportCardDTO?> UpdateReportCardAsync(int reportCardId, string comments)
        {
            var reportCard = await _context.ReportCards
                .Include(r => r.Student)
                .Include(r => r.Teacher)
                .Include(r => r.ReportCardSubjects)
                .ThenInclude(rs => rs.Subject)
                .FirstOrDefaultAsync(r => r.Id == reportCardId);

            if (reportCard == null) return null;

            reportCard.Comments = comments;
            _context.ReportCards.Update(reportCard);
            await _context.SaveChangesAsync();

            return MapToDTO(reportCard, reportCard.Student, reportCard.Teacher);
        }

        private ReportCardDTO MapToDTO(ReportCard reportCard, Student student, Teacher? teacher)
        {
            return new ReportCardDTO
            {
                Id = reportCard.Id,
                StudentId = student.Id,
                StudentAdmissionNumber = student.AdmissionNumber,
                StudentFullName = student.FullName,
                Term = reportCard.Term,
                AcademicYear = reportCard.AcademicYear,
                OverallGPA = reportCard.OverallGPA,
                ClassPosition = reportCard.ClassPosition,
                Status = reportCard.Status,
                Comments = reportCard.Comments,
                TeacherName = teacher?.FullName ?? "N/A",
                GeneratedDate = reportCard.GeneratedDate,
                Subjects = reportCard.ReportCardSubjects
                    .Select(rs => new ReportCardSubjectDTO
                    {
                        SubjectId = rs.SubjectId,
                        SubjectName = rs.Subject.Name,
                        Marks = rs.Marks,
                        LetterGrade = rs.LetterGrade,
                        ClassAverage = rs.ClassAverage
                    })
                    .ToList()
            };
        }
    }
}
