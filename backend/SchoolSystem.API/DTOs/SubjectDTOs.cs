namespace SchoolSystem.API.DTOs
{
    public class SubjectDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string SchoolLevel { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class CreateSubjectDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string SchoolLevel { get; set; } = string.Empty;
    }

    public class UpdateSubjectDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string SchoolLevel { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
