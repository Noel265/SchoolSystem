namespace SchoolSystem.API.Models
{
    public class Attendance
    {
        public int Id { get; set; }
        public DateOnly Date { get; set; }
        public string Status { get; set; } = string.Empty; // "Present", "Absent"
        public string? Remarks { get; set; }

        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;

        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; } = null!;
    }
}