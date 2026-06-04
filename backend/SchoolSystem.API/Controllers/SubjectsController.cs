using Microsoft.AspNetCore.Mvc;
using SchoolSystem.API.DTOs;
using SchoolSystem.API.Services;

namespace SchoolSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubjectsController : ControllerBase
    {
        private readonly SubjectService _subjectService;

        public SubjectsController(SubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateSubject(CreateSubjectDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Code))
                return BadRequest(new { message = "Name and Code are required." });

            var result = await _subjectService.CreateSubjectAsync(dto);
            return CreatedAtAction(nameof(GetSubject), new { id = result?.Id }, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSubject(int id)
        {
            var result = await _subjectService.GetSubjectAsync(id);
            if (result == null)
                return NotFound(new { message = "Subject not found." });

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetSubjects([FromQuery] string? schoolLevel = null)
        {
            var result = await _subjectService.GetSubjectsAsync(schoolLevel);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSubject(int id, UpdateSubjectDTO dto)
        {
            var result = await _subjectService.UpdateSubjectAsync(id, dto);
            if (result == null)
                return NotFound(new { message = "Subject not found." });

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubject(int id)
        {
            var result = await _subjectService.DeleteSubjectAsync(id);
            if (!result)
                return NotFound(new { message = "Subject not found." });

            return NoContent();
        }
    }
}
