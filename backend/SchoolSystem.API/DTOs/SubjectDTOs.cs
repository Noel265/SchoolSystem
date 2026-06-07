namespace SchoolSystem.API.DTOs
{
    public class SubjectResponseDTO
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
        public bool IsActive { get; set; } = true;
    }

    public class ClassSubjectResponseDTO
    {
        public int Id { get; set; }
        public int ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public string SchoolLevel { get; set; } = string.Empty;
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public string SubjectCode { get; set; } = string.Empty;
        public int TeacherId { get; set; }
        public string TeacherName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class AssignClassSubjectDTO
    {
        public int ClassId { get; set; }
        public int SubjectId { get; set; }
        public int TeacherId { get; set; }
    }
}
