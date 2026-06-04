namespace SchoolSystem.API.Models
{
    public class ReportCard
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;

        public int ClassId { get; set; }
        public Class Class { get; set; } = null!;

        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; } = null!;

        public string Term { get; set; } = string.Empty; // "Term 1", "Term 2", "Term 3"
        public string AcademicYear { get; set; } = string.Empty;

        public decimal OverallGPA { get; set; } // Average across all subjects
        public int ClassPosition { get; set; } // Student's rank in class
        public string Status { get; set; } = string.Empty; // "Pass", "Fail"
        public string Comments { get; set; } = string.Empty; // Teacher's comments

        public DateTime GeneratedDate { get; set; } = DateTime.UtcNow;

        public ICollection<ReportCardSubject> ReportCardSubjects { get; set; } = new List<ReportCardSubject>();
    }
}
