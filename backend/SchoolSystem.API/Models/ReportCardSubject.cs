namespace SchoolSystem.API.Models
{
    public class ReportCardSubject
    {
        public int Id { get; set; }
        public int ReportCardId { get; set; }
        public ReportCard ReportCard { get; set; } = null!;

        public int SubjectId { get; set; }
        public Subject Subject { get; set; } = null!;

        public decimal Marks { get; set; } // 0-100
        public string LetterGrade { get; set; } = string.Empty; // A, B, C, D, F
        public decimal ClassAverage { get; set; } // Average for this subject in the class
    }
}
