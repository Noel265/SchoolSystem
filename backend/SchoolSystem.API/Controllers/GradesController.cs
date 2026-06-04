using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public GradesController(GradeService gradeService)
        {
            _gradeService = gradeService;
        }

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
    }
}
