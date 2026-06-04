namespace SchoolSystem.API.DTOs
{
    public class MarkAttendanceDTO
    {
        public int StudentId { get; set; }
        public DateOnly Date { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Remarks { get; set; }
    }

    public class BulkAttendanceDTO
    {
        public int ClassId { get; set; }
        public DateOnly Date { get; set; }
        public List<StudentAttendanceItemDTO> Attendances { get; set; } = new();
    }

    public class StudentAttendanceItemDTO
    {
        public int StudentId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Remarks { get; set; }
    }

    public class AttendanceResponseDTO
    {
        public int Id { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string AdmissionNumber { get; set; } = string.Empty;
        public DateOnly Date { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Remarks { get; set; }
        public string TeacherName { get; set; } = string.Empty;
    }

    public class AttendanceSummaryDTO
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public int TotalDays { get; set; }
        public int PresentDays { get; set; }
        public int AbsentDays { get; set; }
        public int LateDays { get; set; }
        public double AttendancePercentage { get; set; }
    }
}