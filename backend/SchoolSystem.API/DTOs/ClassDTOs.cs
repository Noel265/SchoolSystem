namespace SchoolSystem.API.DTOs
{
    public class CreateClassDTO
    {
        public string Name { get; set; } = string.Empty;
        public string SchoolLevel { get; set; } = string.Empty; // "Primary", "Secondary"
        public string AcademicYear { get; set; } = string.Empty;
        public int? ClassTeacherId { get; set; }
    }

    public class UpdateClassDTO
    {
        public string Name { get; set; } = string.Empty;
        public string SchoolLevel { get; set; } = string.Empty;
        public string AcademicYear { get; set; } = string.Empty;
        public int? ClassTeacherId { get; set; }
    }


    public class ClassResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SchoolLevel { get; set; } = string.Empty;
        public string AcademicYear { get; set; } = string.Empty;
        public string? ClassTeacherName { get; set; }
        public int StudentCount { get; set; }
    }
}