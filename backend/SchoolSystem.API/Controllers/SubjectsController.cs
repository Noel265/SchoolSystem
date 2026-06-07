using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolSystem.API.DTOs;
using SchoolSystem.API.Services;

namespace SchoolSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SubjectsController : ControllerBase
    {
        private readonly SubjectService _subjectService;

        public SubjectsController(SubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _subjectService.GetAllAsync());
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateSubjectDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name) ||
                string.IsNullOrWhiteSpace(dto.Code) ||
                string.IsNullOrWhiteSpace(dto.SchoolLevel))
                return BadRequest(new { message = "Name, code, and school level are required." });

            var validLevels = new[] { "Primary", "Secondary" };
            if (!validLevels.Contains(dto.SchoolLevel))
                return BadRequest(new { message = "SchoolLevel must be Primary or Secondary." });

            var result = await _subjectService.CreateAsync(dto);
            if (result == null)
                return Conflict(new { message = "A subject with this code already exists." });

            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, UpdateSubjectDTO dto)
        {
            var result = await _subjectService.UpdateAsync(id, dto);
            if (result == null)
                return NotFound(new { message = "Subject not found or code already exists." });

            return Ok(result);
        }

        [HttpGet("assignments")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> GetAssignments([FromQuery] int? classId, [FromQuery] int? teacherId)
        {
            return Ok(await _subjectService.GetAssignmentsAsync(classId, teacherId));
        }

        [HttpGet("class/{classId}")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> GetByClass(int classId)
        {
            return Ok(await _subjectService.GetAssignmentsAsync(classId));
        }

        [HttpPost("assignments")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Assign(AssignClassSubjectDTO dto)
        {
            if (dto.ClassId <= 0 || dto.SubjectId <= 0 || dto.TeacherId <= 0)
                return BadRequest(new { message = "Class, subject, and teacher are required." });

            var result = await _subjectService.AssignAsync(dto);
            if (result == null)
                return BadRequest(new { message = "Class, subject, or teacher not found; or subject level does not match class level." });

            return Ok(result);
        }

        [HttpDelete("assignments/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeactivateAssignment(int id)
        {
            var success = await _subjectService.DeactivateAssignmentAsync(id);
            if (!success)
                return NotFound(new { message = "Assignment not found." });

            return Ok(new { message = "Assignment deactivated successfully." });
        }
    }
}
