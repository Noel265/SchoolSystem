namespace SchoolSystem.API.Models
{
    public class TimetableEntry
    {
        public int Id { get; set; }
        public string DayOfWeek { get; set; } = string.Empty; // "Monday", etc
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string? Room { get; set; }

        public int ClassId { get; set; }
        public Class Class { get; set; } = null!;

        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; } = null!;
    }
}