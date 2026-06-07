namespace SchoolSystem.API.Models
{
    public class Subject
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty; // e.g., "ENG", "MATH"
        public string SchoolLevel { get; set; } = string.Empty; // "Primary", "Secondary"
        public bool IsActive { get; set; } = true;

        public ICollection<Grade> Grades { get; set; } = new List<Grade>();
        public ICollection<ClassSubject> ClassSubjects { get; set; } = new List<ClassSubject>();
    }
}
