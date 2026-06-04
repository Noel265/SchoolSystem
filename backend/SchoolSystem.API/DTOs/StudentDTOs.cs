namespace SchoolSystem.API.DTOs
{
    public class CreateStudentDTO
    {
        public string FullName { get; set; } = string.Empty;
        public string AdmissionNumber { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string SchoolLevel { get; set; } = String.Empty;
        public int ClassId { get; set; }
    }

    public class UpdateStudentDTO
    {
        public string FullName { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string SchoolLevel { get; set; } = string.Empty;
        public int ClassId { get; set; }
    }

    public class StudentResponseDTO
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string AdmissionNumber { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string SchoolLevel { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public DateOnly EnrollmentDate { get; set; }
        public bool IsActive { get; set; }
    }
}