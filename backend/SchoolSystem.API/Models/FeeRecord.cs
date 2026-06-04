namespace SchoolSystem.API.Models
{
    public class FeeRecord
    {
        public int Id { get; set; }
        public string FeeType { get; set; } = string.Empty; // "Tuition", "Exam", "Library"
        public decimal AmountDue { get; set; }
        public decimal AmountPaid { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string Status { get; set; } = string.Empty; // "Paid", "Partial", "Unpaid"
        public string Term { get; set; } = string.Empty;
        public string AcademicYear { get; set; } = string.Empty;

        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;
    }
}