namespace SchoolSystem.API.Models
{
    public class Grade
    {
        public int Id { get; set; }
        public string Subject { get; set; } = string.Empty;
        public decimal Score { get; set; }
        public decimal MaxScore { get; set; }
        public string Term { get; set; } = string.Empty; // "Term 1", "Term 2"
        public string AcademicYear { get; set; } = string.Empty;
        public string ExamType { get; set; } = string.Empty; // "Midterm", "Final", "CAT"

        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;

        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; } = null!;
    }
}