using Microsoft.EntityFrameworkCore;
using SchoolSystem.API.Data;
using SchoolSystem.API.Models;

namespace SchoolSystem.API.Services
{
    // This service is deprecated - functionality merged into GradeService
    public class ReportCardService
    {
        private readonly AppDbContext _context;

        public ReportCardService(AppDbContext context)
        {
            _context = context;
        }
    }
}
