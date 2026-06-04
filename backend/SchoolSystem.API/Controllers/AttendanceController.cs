using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolSystem.API.DTOs;
using SchoolSystem.API.Services;
using System.Security.Claims;

namespace SchoolSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AttendanceController : ControllerBase
    {
        private readonly AttendanceService _attendanceService;

        public AttendanceController(AttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        [HttpGet("class/{classId}/date/{date}")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> GetByClassAndDate(int classId, DateOnly date)
        {
            var result = await _attendanceService.GetByClassAndDateAsync(classId, date);
            return Ok(result);
        }

        [HttpGet("student/{studentId}")]
        [Authorize(Roles = "Admin,Teacher,Parent")]
        public async Task<IActionResult> GetByStudent(int studentId)
        {
            var result = await _attendanceService.GetByStudentAsync(studentId);
            return Ok(result);
        }

        [HttpGet("student/{studentId}/summary")]
        [Authorize(Roles = "Admin,Teacher,Parent")]
        public async Task<IActionResult> GetStudentSummary(int studentId)
        {
            var result = await _attendanceService.GetStudentSummaryAsync(studentId);
            if (result == null)
                return NotFound(new { message = "Student not found." });
            return Ok(result);
        }

        [HttpPost("bulk")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> BulkMark(BulkAttendanceDTO dto)
        {
            // Get teacher id from JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();

            var userId = int.Parse(userIdClaim);

            // Get teacher record from user id
            var teacher = await System.Threading.Tasks.Task.Run(async () =>
            {
                using var scope = HttpContext.RequestServices.CreateScope();
                var context = scope.ServiceProvider
                    .GetRequiredService<SchoolSystem.API.Data.AppDbContext>();
                return await context.Teachers
                    .FirstOrDefaultAsync(t => t.UserId == userId);
            });

            if (teacher == null)
                return Forbid();

            var success = await _attendanceService.BulkMarkAsync(dto, teacher.Id);
            if (!success)
                return BadRequest(new { message = "Failed to mark attendance." });

            return Ok(new { message = "Attendance marked successfully." });
        }
    }
}