namespace SchoolSystem.API.Models
{
    public class Parent
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; }= string.Empty;
        public string Address { get; set; } = string.Empty;

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public ICollection<StudentParent> StudentParents { get; set; } = new List<StudentParent>();

    }
}