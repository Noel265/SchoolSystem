using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolSystem.API.DTOs;
using SchoolSystem.API.Services;
using System.Security.Claims;

namespace SchoolSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportCardsController : ControllerBase
    {
        private readonly ReportCardService _reportCardService;

        public ReportCardsController(ReportCardService reportCardService)
        {
            _reportCardService = reportCardService;
        }

        [HttpPost]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> GenerateReportCard(CreateReportCardDTO dto)
        {
            if (dto.StudentId <= 0 || string.IsNullOrWhiteSpace(dto.Term) || string.IsNullOrWhiteSpace(dto.AcademicYear))
                return BadRequest(new { message = "StudentId, Term, and AcademicYear are required." });

            var teacherId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var result = await _reportCardService.GenerateReportCardAsync(dto, teacherId);

            if (result == null)
                return NotFound(new { message = "Student not found or no grades available for the specified term." });

            return CreatedAtAction(nameof(GetReportCard), new { id = result.Id }, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReportCard(int id)
        {
            var result = await _reportCardService.GetReportCardAsync(id);
            if (result == null)
                return NotFound(new { message = "Report card not found." });

            return Ok(result);
        }

        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetStudentReportCards(int studentId)
        {
            var result = await _reportCardService.GetStudentReportCardsAsync(studentId);
            return Ok(result);
        }

        [HttpGet("class/{classId}")]
        public async Task<IActionResult> GetClassReportCards(int classId, [FromQuery] string term, [FromQuery] string academicYear)
        {
            if (string.IsNullOrWhiteSpace(term) || string.IsNullOrWhiteSpace(academicYear))
                return BadRequest(new { message = "Term and AcademicYear are required." });

            var result = await _reportCardService.GetClassReportCardsAsync(classId, term, academicYear);
            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> UpdateReportCard(int id, [FromBody] dynamic request)
        {
            var comments = request?.comments?.ToString() ?? string.Empty;
            var result = await _reportCardService.UpdateReportCardAsync(id, comments);

            if (result == null)
                return NotFound(new { message = "Report card not found." });

            return Ok(result);
        }
    }
}
