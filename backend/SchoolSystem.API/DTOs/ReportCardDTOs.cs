namespace SchoolSystem.API.DTOs
{
    public class ReportCardDTO
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string StudentAdmissionNumber { get; set; } = string.Empty;
        public string StudentFullName { get; set; } = string.Empty;
        public string Term { get; set; } = string.Empty;
        public string AcademicYear { get; set; } = string.Empty;
        public decimal OverallGPA { get; set; }
        public int ClassPosition { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Comments { get; set; } = string.Empty;
        public string TeacherName { get; set; } = string.Empty;
        public DateTime GeneratedDate { get; set; }
        public List<ReportCardSubjectDTO> Subjects { get; set; } = new List<ReportCardSubjectDTO>();
    }

    public class CreateReportCardDTO
    {
        public int StudentId { get; set; }
        public string Term { get; set; } = string.Empty;
        public string AcademicYear { get; set; } = string.Empty;
        public string Comments { get; set; } = string.Empty;
    }

    public class ReportCardSubjectDTO
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public decimal Marks { get; set; }
        public string LetterGrade { get; set; } = string.Empty;
        public decimal ClassAverage { get; set; }
    }
}
