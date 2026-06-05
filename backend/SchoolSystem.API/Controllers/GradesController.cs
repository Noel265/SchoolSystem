using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolSystem.API.Data;
using SchoolSystem.API.DTOs;
using SchoolSystem.API.Services;
using System.Security.Claims;

namespace SchoolSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GradesController : ControllerBase
    {
        private readonly GradeService _gradeService;
        private readonly AppDbContext _context;

        public GradesController(GradeService gradeService, AppDbContext context)
        {
            _gradeService = gradeService;
            _context = context;
        }

        // ── Legacy Endpoints (Keep for backward compatibility) ────────────

        [HttpGet("student/{admissionNumber}")]
        public async Task<IActionResult> GetStudentGrades(string admissionNumber, [FromQuery] string term, [FromQuery] string academicYear)
        {
            if (string.IsNullOrWhiteSpace(term) || string.IsNullOrWhiteSpace(academicYear))
                return BadRequest(new { message = "Term and AcademicYear are required." });

            var result = await _gradeService.GetStudentByAdmissionNumberAsync(admissionNumber, term, academicYear);
            if (result == null)
                return NotFound(new { message = "Student not found." });

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> CreateGrade(CreateGradeDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.StudentAdmissionNumber) || dto.SubjectId <= 0 || dto.Marks < 0 || dto.Marks > 100)
                return BadRequest(new { message = "Invalid input. Marks must be between 0 and 100." });

            var teacherId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var result = await _gradeService.CreateGradeAsync(dto, teacherId);

            if (result == null)
                return NotFound(new { message = "Student or Subject not found." });

            return CreatedAtAction(nameof(GetStudentGrades), new { admissionNumber = dto.StudentAdmissionNumber }, result);
        }

        [HttpPut("{gradeId}")]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> UpdateGrade(int gradeId, UpdateGradeDTO dto)
        {
            if (dto.Marks < 0 || dto.Marks > 100)
                return BadRequest(new { message = "Marks must be between 0 and 100." });

            var teacherId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var result = await _gradeService.UpdateGradeAsync(gradeId, dto, teacherId);

            if (result == null)
                return NotFound(new { message = "Grade not found." });

            return Ok(result);
        }

        [HttpGet("class/{classId}")]
        public async Task<IActionResult> GetClassGrades(int classId, [FromQuery] string term, [FromQuery] string academicYear)
        {
            if (string.IsNullOrWhiteSpace(term) || string.IsNullOrWhiteSpace(academicYear))
                return BadRequest(new { message = "Term and AcademicYear are required." });

            var result = await _gradeService.GetClassGradesAsync(classId, term, academicYear);
            return Ok(result);
        }

        // ── NEW: Midterm/Final Workflow ────────────────────────────────

        [HttpGet("lookup/{registrationNumber}")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> LookupStudent(string registrationNumber)
        {
            var student = await _gradeService.LookupStudentAsync(registrationNumber);
            if (student == null)
                return NotFound(new { message = "Student not found." });
            return Ok(student);
        }

        [HttpPost("enter")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> EnterGrade(EnterGradeDTO dto)
        {
            if (dto.MidtermScore < 0 || dto.MidtermScore > 50)
                return BadRequest(new { message = "Midterm score must be between 0 and 50." });

            if (dto.FinalScore < 0 || dto.FinalScore > 50)
                return BadRequest(new { message = "Final score must be between 0 and 50." });

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) return Unauthorized();

            var userId = int.Parse(userIdClaim);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            int teacherId;
            if (userRole == "Admin")
            {
                teacherId = 0;
            }
            else
            {
                var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.UserId == userId);
                if (teacher == null) return Forbid();
                teacherId = teacher.Id;
            }

            var result = await _gradeService.EnterGradeAsync(dto, teacherId);
            if (result == null)
                return BadRequest(new { message = "Student or subject not found, or invalid scores." });

            return Ok(result);
        }

        [HttpGet("student/{registrationNumber}/subjects")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> GetAvailableSubjects(string registrationNumber)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) return Unauthorized();

            int? teacherId = null;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole == "Teacher")
            {
                var userId = int.Parse(userIdClaim);
                var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.UserId == userId);
                if (teacher == null) return Forbid();
                teacherId = teacher.Id;
            }

            var result = await _gradeService.GetAvailableSubjectsForStudentAsync(registrationNumber, teacherId);
            return Ok(result);
        }

        [HttpGet("student/{registrationNumber}/table")]
        [Authorize(Roles = "Admin,Teacher,Parent")]
        public async Task<IActionResult> GetStudentGradesTable(
            string registrationNumber, [FromQuery] string term, [FromQuery] string academicYear)
        {
            if (string.IsNullOrWhiteSpace(term) || string.IsNullOrWhiteSpace(academicYear))
                return BadRequest(new { message = "Term and AcademicYear are required." });

            var result = await _gradeService.GetStudentGradesTableAsync(registrationNumber, term, academicYear);
            if (result == null)
                return NotFound(new { message = "Student not found." });

            return Ok(result);
        }

        [HttpGet("reportcard/{registrationNumber}/{term}/{academicYear}")]
        [Authorize(Roles = "Admin,Teacher,Parent")]
        public async Task<IActionResult> GetReportCard(
            string registrationNumber, string term, string academicYear)
        {
            var result = await _gradeService.GetReportCardAsync(registrationNumber, term, academicYear);

            if (result == null)
                return NotFound(new { message = "No grades found for this student." });

            return Ok(result);
        }
    }
}
