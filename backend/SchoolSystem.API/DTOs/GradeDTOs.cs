namespace SchoolSystem.API.DTOs
{
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
}
