using Microsoft.AspNetCore.SignalR;

namespace SchoolSystem.API.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string AdmissionNumber { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string SchoolLevel { get; set; } = string.Empty; // "Primary", "Secondary"
        public DateOnly EnrollmentDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
        public bool IsActive { get; set; } = true;

        public int ClassId { get; set; }
        public Class Class { get; set; } = null!;

        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
        public ICollection<Grade> Grades { get; set; } = new List<Grade>();
        public ICollection<FeeRecord> FeeRecords { get; set; } = new List<FeeRecord>();
        public ICollection<StudentParent> StudentParents { get; set; } = new List<StudentParent>();
    }
}