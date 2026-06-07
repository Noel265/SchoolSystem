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
    public class ParentController : ControllerBase
    {
        private readonly ParentService _parentService;

        public ParentController(ParentService parentService)
        {
            _parentService = parentService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var parents = await _parentService.GetAllAsync();
            return Ok(parents);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _parentService.GetByIdAsync(id);
            if (result == null)
                return NotFound(new { message = "Parent not found." });

            return Ok(result);
        }

        [HttpGet("my-student")]
        [Authorize(Roles = "Parent")]
        public async Task<IActionResult> GetMyStudent()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) return Unauthorized();

            var userId = int.Parse(userIdClaim);
            var result = await _parentService.GetStudentByParentUserIdAsync(userId);

            if (result == null)
                return NotFound(new { message = "No student linked to this account." });

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateParentDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FullName) ||
                string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Password) ||
                string.IsNullOrWhiteSpace(dto.Relationship) ||
                dto.StudentId == 0)
                return BadRequest(new { message = "All fields are required." });

            var validRelationships = new[] { "Mother", "Father", "Guardian" };
            if (!validRelationships.Contains(dto.Relationship))
                return BadRequest(new { message = "Relationship must be Mother, Father, or Guardian." });

            var result = await _parentService.CreateAsync(dto);
            if (result == null)
                return Conflict(new { message = "Email already exists or student not found." });

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Deactivate(int id)
        {
            var success = await _parentService.DeactivateAsync(id);
            if (!success)
                return NotFound(new { message = "Parent not found." });
            return Ok(new { message = "Parent deactivated successfully." });
        }
    }
}