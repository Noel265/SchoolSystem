namespace SchoolSystem.API.Models
{
    public class Class
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; //  e.g. "Standard 4", "Form 2"
        public string SchoolLevel { get; set; } = string.Empty; // "Primary", "Secondary"
        public string AcademicYear { get; set; } = string.Empty; // e.g. "2025"

        public int? ClassTeacherId {get; set; }
        public Teacher? ClassTeacher {get; set; }

        public ICollection<Student> Students {get; set; } = new List<Student>();
        public ICollection<TimetableEntry> TimetableEntries {get; set; } = new List<TimetableEntry>();




    }
}