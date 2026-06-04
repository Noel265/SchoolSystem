using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolSystem.API.DTOs;
using SchoolSystem.API.Services;

namespace SchoolSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StudentsController : ControllerBase
    {
        private readonly StudentService _studentService;

        public StudentsController(StudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> GetAll()
        {
            var students = await _studentService.GetAllAsync();
            return Ok(students);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var student = await _studentService.GetByIdAsync(id);
            if (student == null)
                return  NotFound(new { message = "Student not found." });
            return Ok(student);    
        }

        [HttpGet("class/{classId}")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> GetByClass(int classId)
        {
            var students = await _studentService.GetByClassAsync(classId);
            return Ok(students);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateStudentDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FullName) ||
                string.IsNullOrWhiteSpace(dto.AdmissionNumber) ||
                string.IsNullOrWhiteSpace(dto.Gender) ||
                string.IsNullOrWhiteSpace(dto.SchoolLevel))
                return BadRequest(new {message = "All fields are required."});

            var validLevels = new[] { "Primary", "Secondary" };
            if (!validLevels.Contains(dto.SchoolLevel))
                return BadRequest(new {message = "SchoolLevel must be Primary or Secondary." });
            
            var result = await _studentService.CreateAsync(dto);
            if (result == null)
                return Conflict(new { message = "Admission number already exists or class not found." });
            
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, UpdateStudentDTO dto)
        {
            var result = await _studentService.UpdateAsync(id, dto);
            if (result == null)
                return NotFound(new { message = "Student or class not found." });
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Deactivate(int id)
        {
            var success = await _studentService.DeactivateAsync(id);
            if (!success)
                return NotFound(new { message = "Student not found." });
            return Ok(new { message = "Student deactivated successfully." });
        }
    }
}