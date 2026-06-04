namespace SchoolSystem.API.Models
{
    public class Teacher
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string EmployeeNumber { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public DateOnly HireDate { get; set; }
        public bool IsActive { get; set; } = true;

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public ICollection<Class> Classes { get; set; } = new List<Class>();
        public ICollection<TimetableEntry> TimetableEntries { get; set; } = new List<TimetableEntry>();
    }
}