namespace SchoolSystem.API.Models
{
    public class Grade
    {
        public int Id { get; set; }
        public int SubjectId { get; set; }
        public Subject Subject { get; set; } = null!;

        // Legacy: 0-100 marks
        public decimal Marks { get; set; } // 0-100
        public string LetterGrade { get; set; } = string.Empty; // A, B, C, D, F

        // New: Midterm + Final scores (0-50 each)
        public decimal MidtermScore { get; set; } = 0;
        public decimal FinalScore { get; set; } = 0;
        public decimal TotalScore => MidtermScore + FinalScore; // Computed property (not stored)

        public string Term { get; set; } = string.Empty; // "Term 1", "Term 2", "Term 3"
        public string AcademicYear { get; set; } = string.Empty;
        public string? Comments { get; set; } // Teacher comments

        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;

        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; } = null!;

        public DateTime EnteredDate { get; set; } = DateTime.UtcNow;
    }
}