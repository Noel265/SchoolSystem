using Microsoft.EntityFrameworkCore;
using SchoolSystem.API.Models;

namespace SchoolSystem.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Parent> Parents { get; set; }
        public DbSet<StudentParent> StudentParents { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<ReportCard> ReportCards { get; set; }
        public DbSet<ReportCardSubject> ReportCardSubjects { get; set; }
        public DbSet<FeeRecord> FeeRecords { get; set; }
        public DbSet<TimetableEntry> TimetableEntries { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Composite key for StudentParent join table
            modelBuilder.Entity<StudentParent>()
                .HasKey(sp => new { sp.StudentId, sp.ParentId });

            // Student → Class
            modelBuilder.Entity<Student>()
                .HasOne(s => s.Class)
                .WithMany(c => c.Students)
                .HasForeignKey(s => s.ClassId)
                .OnDelete(DeleteBehavior.Restrict);

            // Teacher → User (one-to-one)
            modelBuilder.Entity<Teacher>()
                .HasOne(t => t.User)
                .WithOne(u => u.Teacher)
                .HasForeignKey<Teacher>(t => t.UserId);

            // Parent → User (one-to-one)
            modelBuilder.Entity<Parent>()
                .HasOne(p => p.User)
                .WithOne(u => u.Parent)
                .HasForeignKey<Parent>(p => p.UserId);

            // Grade → TotalScore is computed property (not stored in DB)
            modelBuilder.Entity<Grade>()
                .Ignore(g => g.TotalScore);

            // Unique constraints
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email).IsUnique();

            modelBuilder.Entity<Student>()
                .HasIndex(s => s.AdmissionNumber).IsUnique();

            modelBuilder.Entity<Teacher>()
                .HasIndex(t => t.EmployeeNumber).IsUnique();
        }
    }
}