namespace SchoolSystem.API.DTOs
{
    // ── Legacy DTOs (Keep for backward compatibility) ──────────────

    public class GradeDTO
    {
        public int Id { get; set; }
        public string StudentAdmissionNumber { get; set; } = string.Empty;
        public string StudentFullName { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public decimal Marks { get; set; }
        public string LetterGrade { get; set; } = string.Empty;
        public string Term { get; set; } = string.Empty;
        public string AcademicYear { get; set; } = string.Empty;
        public string TeacherName { get; set; } = string.Empty;
        public DateTime EnteredDate { get; set; }
    }

    public class CreateGradeDTO
    {
        public string StudentAdmissionNumber { get; set; } = string.Empty;
        public int SubjectId { get; set; }
        public decimal Marks { get; set; }
        public string Term { get; set; } = string.Empty;
        public string AcademicYear { get; set; } = string.Empty;
    }

    public class UpdateGradeDTO
    {
        public decimal Marks { get; set; }
    }

    public class StudentGradesDTO
    {
        public int StudentId { get; set; }
        public string StudentAdmissionNumber { get; set; } = string.Empty;
        public string StudentFullName { get; set; } = string.Empty;
        public int StudentAge { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public List<SubjectGradeDTO> Subjects { get; set; } = new List<SubjectGradeDTO>();
        public decimal ClassAverage { get; set; }
        public string Status { get; set; } = string.Empty;
        public int Position { get; set; }
        public string TeacherName { get; set; } = string.Empty;
    }

    public class SubjectGradeDTO
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public decimal Marks { get; set; }
        public string LetterGrade { get; set; } = string.Empty;
        public decimal ClassAverage { get; set; }
    }

    // ── New DTOs (Midterm + Final workflow) ──────────────────────

    public class EnterGradeDTO
    {
        public string RegistrationNumber { get; set; } = string.Empty;
        public int SubjectId { get; set; }
        public decimal MidtermScore { get; set; }
        public decimal FinalScore { get; set; }
        public string Term { get; set; } = string.Empty;
        public string AcademicYear { get; set; } = string.Empty;
        public string? Comments { get; set; }
    }

    public class GradeResponseDTO
    {
        public int Id { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string RegistrationNumber { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public decimal MidtermScore { get; set; }
        public decimal FinalScore { get; set; }
        public decimal TotalScore { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Term { get; set; } = string.Empty;
        public string AcademicYear { get; set; } = string.Empty;
        public string? Comments { get; set; }
        public string TeacherName { get; set; } = string.Empty;
    }

    public class StudentGradesTableDTO
    {
        public string StudentName { get; set; } = string.Empty;
        public string RegistrationNumber { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string Term { get; set; } = string.Empty;
        public string AcademicYear { get; set; } = string.Empty;
        public List<GradeResponseDTO> Grades { get; set; } = new();
    }

    public class ReportCardSubjectDTO
    {
        public string SubjectName { get; set; } = string.Empty;
        public decimal MidtermScore { get; set; }
        public decimal FinalScore { get; set; }
        public decimal SubjectTotal { get; set; }
        public decimal ClassMidtermAverage { get; set; }
        public decimal ClassFinalAverage { get; set; }
        public decimal ClassSubjectAverage { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Comments { get; set; }
        public string TeacherName { get; set; } = string.Empty;
    }

    public class ReportCardDTO
    {
        public string StudentName { get; set; } = string.Empty;
        public string RegistrationNumber { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Term { get; set; } = string.Empty;
        public string AcademicYear { get; set; } = string.Empty;
        public List<ReportCardSubjectDTO> Subjects { get; set; } = new();
        public decimal MidtermTotal { get; set; }
        public decimal FinalTotal { get; set; }
        public decimal OverallAverage { get; set; }
        public int Position { get; set; }
        public int TotalStudentsInClass { get; set; }
        public string OverallStatus { get; set; } = string.Empty;
    }
}
