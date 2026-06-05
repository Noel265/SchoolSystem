using Microsoft.EntityFrameworkCore;
using SchoolSystem.API.Data;
using SchoolSystem.API.DTOs;
using SchoolSystem.API.Models;

namespace SchoolSystem.API.Services
{
    public class GradeService
    {
        private readonly AppDbContext _context;
        private const decimal PASS_THRESHOLD = 50m;

        public GradeService(AppDbContext context)
        {
            _context = context;
        }

        // Get student by admission number with age
        public async Task<StudentGradesDTO?> GetStudentByAdmissionNumberAsync(string admissionNumber, string term, string academicYear)
        {
            var student = await _context.Students
                .Include(s => s.Class)
                .Include(s => s.Grades.Where(g => g.Term == term && g.AcademicYear == academicYear))
                .ThenInclude(g => g.Subject)
                .FirstOrDefaultAsync(s => s.AdmissionNumber == admissionNumber);

            if (student == null) return null;

            var grades = student.Grades.ToList();
            var subjectGrades = grades
                .GroupBy(g => g.SubjectId)
                .Select(g => new SubjectGradeDTO
                {
                    SubjectId = g.Key,
                    SubjectName = g.First().Subject.Name,
                    Marks = g.First().Marks,
                    LetterGrade = g.First().LetterGrade,
                    ClassAverage = GetClassAverageForSubject(g.Key, term, academicYear)
                })
                .ToList();

            var classAverage = grades.Any() ? grades.Average(g => g.Marks) : 0;
            var position = CalculateStudentPosition(student.ClassId, student.Id, term, academicYear);
            var status = DetermineStatus(classAverage);

            var classTeacher = await _context.Teachers.FindAsync(student.Class.ClassTeacherId);

            return new StudentGradesDTO
            {
                StudentId = student.Id,
                StudentAdmissionNumber = student.AdmissionNumber,
                StudentFullName = student.FullName,
                StudentAge = CalculateAge(student.DateOfBirth),
                ClassName = student.Class.Name,
                Subjects = subjectGrades,
                ClassAverage = classAverage,
                Status = status,
                Position = position,
                TeacherName = classTeacher?.FullName ?? "N/A"
            };
        }

        // Create grade for student
        public async Task<GradeDTO?> CreateGradeAsync(CreateGradeDTO dto, int teacherId)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.AdmissionNumber == dto.StudentAdmissionNumber);
            if (student == null) return null;

            var subject = await _context.Subjects.FindAsync(dto.SubjectId);
            if (subject == null) return null;

            var letterGrade = CalculateLetterGrade(dto.Marks);

            var grade = new Grade
            {
                StudentId = student.Id,
                SubjectId = dto.SubjectId,
                Marks = dto.Marks,
                LetterGrade = letterGrade,
                Term = dto.Term,
                AcademicYear = dto.AcademicYear,
                TeacherId = teacherId,
                EnteredDate = DateTime.UtcNow
            };

            _context.Grades.Add(grade);
            await _context.SaveChangesAsync();

            return new GradeDTO
            {
                Id = grade.Id,
                StudentAdmissionNumber = student.AdmissionNumber,
                StudentFullName = student.FullName,
                SubjectName = subject.Name,
                Marks = grade.Marks,
                LetterGrade = grade.LetterGrade,
                Term = grade.Term,
                AcademicYear = grade.AcademicYear,
                TeacherName = (await _context.Teachers.FindAsync(teacherId))?.FullName ?? "N/A",
                EnteredDate = grade.EnteredDate
            };
        }

        // Update grade
        public async Task<GradeDTO?> UpdateGradeAsync(int gradeId, UpdateGradeDTO dto, int teacherId)
        {
            var grade = await _context.Grades
                .Include(g => g.Student)
                .Include(g => g.Subject)
                .FirstOrDefaultAsync(g => g.Id == gradeId);

            if (grade == null) return null;

            grade.Marks = dto.Marks;
            grade.LetterGrade = CalculateLetterGrade(dto.Marks);

            _context.Grades.Update(grade);
            await _context.SaveChangesAsync();

            return new GradeDTO
            {
                Id = grade.Id,
                StudentAdmissionNumber = grade.Student.AdmissionNumber,
                StudentFullName = grade.Student.FullName,
                SubjectName = grade.Subject.Name,
                Marks = grade.Marks,
                LetterGrade = grade.LetterGrade,
                Term = grade.Term,
                AcademicYear = grade.AcademicYear,
                TeacherName = (await _context.Teachers.FindAsync(teacherId))?.FullName ?? "N/A",
                EnteredDate = grade.EnteredDate
            };
        }

        // Get all grades for a class in a term
        public async Task<List<StudentGradesDTO>> GetClassGradesAsync(int classId, string term, string academicYear)
        {
            var students = await _context.Students
                .Where(s => s.ClassId == classId)
                .Include(s => s.Class)
                .Include(s => s.Grades.Where(g => g.Term == term && g.AcademicYear == academicYear))
                .ThenInclude(g => g.Subject)
                .ToListAsync();

            var result = new List<StudentGradesDTO>();

            foreach (var student in students)
            {
                var grades = student.Grades.ToList();
                var subjectGrades = grades
                    .GroupBy(g => g.SubjectId)
                    .Select(g => new SubjectGradeDTO
                    {
                        SubjectId = g.Key,
                        SubjectName = g.First().Subject.Name,
                        Marks = g.First().Marks,
                        LetterGrade = g.First().LetterGrade,
                        ClassAverage = GetClassAverageForSubject(g.Key, term, academicYear)
                    })
                    .ToList();

                var classAverage = grades.Any() ? grades.Average(g => g.Marks) : 0;
                var status = DetermineStatus(classAverage);

                result.Add(new StudentGradesDTO
                {
                    StudentId = student.Id,
                    StudentAdmissionNumber = student.AdmissionNumber,
                    StudentFullName = student.FullName,
                    StudentAge = CalculateAge(student.DateOfBirth),
                    ClassName = student.Class.Name,
                    Subjects = subjectGrades,
                    ClassAverage = classAverage,
                    Status = status,
                    Position = result.Count + 1, // Will be recalculated below
                    TeacherName = student.Class.ClassTeacher?.FullName ?? "N/A"
                });
            }

            // Assign correct positions based on class average (highest first)
            var sortedByAverage = result.OrderByDescending(r => r.ClassAverage).ToList();
            for (int i = 0; i < sortedByAverage.Count; i++)
            {
                var dto = result.First(r => r.StudentId == sortedByAverage[i].StudentId);
                dto.Position = i + 1;
            }

            return result;
        }

        private string CalculateLetterGrade(decimal marks)
        {
            return marks switch
            {
                >= 90 => "A",
                >= 80 => "B",
                >= 70 => "C",
                >= 60 => "D",
                _ => "F"
            };
        }

        private string DetermineStatus(decimal classAverage)
        {
            return classAverage >= PASS_THRESHOLD ? "Pass" : "Fail";
        }

        private int CalculateStudentPosition(int classId, int studentId, string term, string academicYear)
        {
            var classGrades = _context.Students
                .Where(s => s.ClassId == classId)
                .Include(s => s.Grades.Where(g => g.Term == term && g.AcademicYear == academicYear))
                .ToList();

            var studentAverages = classGrades
                .Select(s => new
                {
                    StudentId = s.Id,
                    Average = s.Grades.Any() ? s.Grades.Average(g => g.Marks) : 0
                })
                .OrderByDescending(x => x.Average)
                .ToList();

            return studentAverages.FindIndex(x => x.StudentId == studentId) + 1;
        }

        private decimal GetClassAverageForSubject(int subjectId, string term, string academicYear)
        {
            var subjectGrades = _context.Grades
                .Where(g => g.SubjectId == subjectId && g.Term == term && g.AcademicYear == academicYear)
                .ToList();

            return subjectGrades.Any() ? subjectGrades.Average(g => g.Marks) : 0;
        }

        private int CalculateAge(DateOnly dateOfBirth)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var age = today.Year - dateOfBirth.Year;
            if (dateOfBirth > today.AddYears(-age)) age--;
            return age;
        }

        // ── NEW: Midterm/Final Workflow ────────────────────────────────

        public async Task<dynamic?> LookupStudentAsync(string registrationNumber)
        {
            var student = await _context.Students
                .Include(s => s.Class)
                .FirstOrDefaultAsync(s =>
                    s.AdmissionNumber == registrationNumber && s.IsActive);

            if (student == null) return null;

            var age = CalculateAge(student.DateOfBirth);

            return new
            {
                student.Id,
                student.FullName,
                student.AdmissionNumber,
                Age = age,
                ClassName = student.Class.Name,
                student.SchoolLevel
            };
        }

        public async Task<List<ClassSubjectResponseDTO>> GetAvailableSubjectsForStudentAsync(
            string registrationNumber, int? teacherId)
        {
            var student = await _context.Students
                .FirstOrDefaultAsync(s =>
                    s.AdmissionNumber == registrationNumber && s.IsActive);

            if (student == null) return new List<ClassSubjectResponseDTO>();

            var query = _context.ClassSubjects
                .Include(cs => cs.Class)
                .Include(cs => cs.Subject)
                .Include(cs => cs.Teacher)
                .Where(cs => cs.IsActive && cs.ClassId == student.ClassId);

            if (teacherId.HasValue)
                query = query.Where(cs => cs.TeacherId == teacherId.Value);

            return await query
                .OrderBy(cs => cs.Subject.Name)
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

        public async Task<GradeResponseDTO?> EnterGradeAsync(EnterGradeDTO dto, int teacherId)
        {
            var student = await _context.Students
                .FirstOrDefaultAsync(s =>
                    s.AdmissionNumber == dto.RegistrationNumber && s.IsActive);

            if (student == null) return null;

            var subject = await _context.Subjects.FindAsync(dto.SubjectId);
            if (subject == null) return null;

            var assignment = await _context.ClassSubjects
                .FirstOrDefaultAsync(cs =>
                    cs.IsActive &&
                    cs.ClassId == student.ClassId &&
                    cs.SubjectId == dto.SubjectId &&
                    cs.TeacherId == teacherId);

            if (assignment == null) return null;

            if (dto.MidtermScore < 0 || dto.MidtermScore > 50 ||
                dto.FinalScore < 0 || dto.FinalScore > 50)
                return null;

            var existing = await _context.Grades
                .FirstOrDefaultAsync(g =>
                    g.StudentId == student.Id &&
                    g.SubjectId == dto.SubjectId &&
                    g.Term == dto.Term &&
                    g.AcademicYear == dto.AcademicYear);

            if (existing != null)
            {
                existing.MidtermScore = dto.MidtermScore;
                existing.FinalScore = dto.FinalScore;
                existing.Comments = dto.Comments;
                existing.TeacherId = teacherId;
            }
            else
            {
                _context.Grades.Add(new Grade
                {
                    StudentId = student.Id,
                    SubjectId = dto.SubjectId,
                    MidtermScore = dto.MidtermScore,
                    FinalScore = dto.FinalScore,
                    Term = dto.Term,
                    AcademicYear = dto.AcademicYear,
                    Comments = dto.Comments,
                    TeacherId = teacherId
                });
            }

            await _context.SaveChangesAsync();

            var teacher = await _context.Teachers.FindAsync(teacherId);
            var total = dto.MidtermScore + dto.FinalScore;

            return new GradeResponseDTO
            {
                Id = existing?.Id ?? (await _context.Grades.MaxAsync(g => g.Id)),
                StudentName = student.FullName,
                RegistrationNumber = student.AdmissionNumber,
                SubjectName = subject.Name,
                MidtermScore = dto.MidtermScore,
                FinalScore = dto.FinalScore,
                TotalScore = total,
                Status = total >= 50 ? "Pass" : "Fail",
                Term = dto.Term,
                AcademicYear = dto.AcademicYear,
                Comments = dto.Comments,
                TeacherName = teacher?.FullName ?? "N/A"
            };
        }

        public async Task<StudentGradesTableDTO?> GetStudentGradesTableAsync(
            string registrationNumber, string term, string academicYear)
        {
            var student = await _context.Students
                .Include(s => s.Class)
                .FirstOrDefaultAsync(s =>
                    s.AdmissionNumber == registrationNumber && s.IsActive);

            if (student == null) return null;

            var grades = await _context.Grades
                .Include(g => g.Subject)
                .Include(g => g.Teacher)
                .Where(g =>
                    g.StudentId == student.Id &&
                    g.Term == term &&
                    g.AcademicYear == academicYear)
                .OrderBy(g => g.Subject.Name)
                .Select(g => new GradeResponseDTO
                {
                    Id = g.Id,
                    StudentName = student.FullName,
                    RegistrationNumber = student.AdmissionNumber,
                    SubjectName = g.Subject.Name,
                    MidtermScore = g.MidtermScore,
                    FinalScore = g.FinalScore,
                    TotalScore = g.MidtermScore + g.FinalScore,
                    Status = g.MidtermScore + g.FinalScore >= 50 ? "Pass" : "Fail",
                    Term = g.Term,
                    AcademicYear = g.AcademicYear,
                    Comments = g.Comments,
                    TeacherName = g.Teacher.FullName
                })
                .ToListAsync();

            return new StudentGradesTableDTO
            {
                StudentName = student.FullName,
                RegistrationNumber = student.AdmissionNumber,
                ClassName = student.Class.Name,
                Term = term,
                AcademicYear = academicYear,
                Grades = grades
            };
        }

        public async Task<ReportCardDTO?> GetReportCardAsync(
            string registrationNumber, string term, string academicYear)
        {
            var student = await _context.Students
                .Include(s => s.Class)
                .FirstOrDefaultAsync(s =>
                    s.AdmissionNumber == registrationNumber && s.IsActive);

            if (student == null) return null;

            var grades = await _context.Grades
                .Include(g => g.Subject)
                .Include(g => g.Teacher)
                .Where(g =>
                    g.StudentId == student.Id &&
                    g.Term == term &&
                    g.AcademicYear == academicYear)
                .ToListAsync();

            if (!grades.Any()) return null;

            // Build subject rows
            var subjectRows = new List<ReportCardSubjectDTO>();

            foreach (var grade in grades)
            {
                var classGradesForSubject = await _context.Grades
                    .Where(g =>
                        g.SubjectId == grade.SubjectId &&
                        g.Term == term &&
                        g.AcademicYear == academicYear &&
                        g.Student.ClassId == student.ClassId)
                    .ToListAsync();

                var classMidtermAvg = classGradesForSubject.Any()
                    ? Math.Round(classGradesForSubject.Average(g => g.MidtermScore), 1)
                    : 0;

                var classFinalAvg = classGradesForSubject.Any()
                    ? Math.Round(classGradesForSubject.Average(g => g.FinalScore), 1)
                    : 0;

                var classSubjectAvg = classGradesForSubject.Any()
                    ? Math.Round(classGradesForSubject.Average(g => g.TotalScore), 1)
                    : 0;

                var subjectTotal = grade.TotalScore;

                subjectRows.Add(new ReportCardSubjectDTO
                {
                    SubjectName = grade.Subject.Name,
                    MidtermScore = grade.MidtermScore,
                    FinalScore = grade.FinalScore,
                    SubjectTotal = subjectTotal,
                    ClassMidtermAverage = classMidtermAvg,
                    ClassFinalAverage = classFinalAvg,
                    ClassSubjectAverage = classSubjectAvg,
                    Status = subjectTotal >= 50 ? "Pass" : "Fail",
                    Comments = grade.Comments,
                    TeacherName = grade.Teacher.FullName
                });
            }

            // Student totals
            var midtermTotal = grades.Sum(g => g.MidtermScore);
            var finalTotal = grades.Sum(g => g.FinalScore);
            var overallAverage = Math.Round((midtermTotal + finalTotal) / grades.Count, 1);

            // Position based on Final Total
            var allStudentTotals = await _context.Grades
                .Where(g =>
                    g.Term == term &&
                    g.AcademicYear == academicYear &&
                    g.Student.ClassId == student.ClassId)
                .GroupBy(g => g.StudentId)
                .Select(grp => new
                {
                    StudentId = grp.Key,
                    FinalTotal = grp.Sum(g => g.FinalScore)
                })
                .OrderByDescending(x => x.FinalTotal)
                .ToListAsync();

            var position = allStudentTotals.FindIndex(x => x.StudentId == student.Id) + 1;
            var totalStudents = allStudentTotals.Count;

            // Overall status (pass if passed majority of subjects)
            var passedSubjects = grades.Count(g => g.TotalScore >= 50);
            var overallStatus = passedSubjects >= Math.Ceiling(grades.Count / 2.0) ? "Pass" : "Fail";

            // Age
            var age = CalculateAge(student.DateOfBirth);

            return new ReportCardDTO
            {
                StudentName = student.FullName,
                RegistrationNumber = student.AdmissionNumber,
                ClassName = student.Class.Name,
                Age = age,
                Term = term,
                AcademicYear = academicYear,
                Subjects = subjectRows,
                MidtermTotal = midtermTotal,
                FinalTotal = finalTotal,
                OverallAverage = overallAverage,
                Position = position,
                TotalStudentsInClass = totalStudents,
                OverallStatus = overallStatus
            };            
        }

        public async Task<List<SubjectResponseDTO>> GetAllSubjectsAsync()
        {
            return await _context.Subjects
                .Where(s => s.IsActive)
                .Select(s => new SubjectResponseDTO
                {
                    Id = s.Id,
                    Name = s.Name,
                    SchoolLevel = s.SchoolLevel,
                    IsActive = s.IsActive
                })
                .ToListAsync();
        }
    }
}
