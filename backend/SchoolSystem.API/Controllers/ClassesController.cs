using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolSystem.API.DTOs;
using SchoolSystem.API.Services;

namespace SchoolSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ClassesController : ControllerBase
    {
        private readonly ClassService _classService;

        public ClassesController(ClassService classService)
        {
            _classService = classService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task <IActionResult> GetAll()
        {
            var classes = await _classService.GetAllAsync();
            return Ok(classes);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _classService.GetByIdAsync(id);
            if (result == null)
                return NotFound( new { Message = "Class not found" });
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateClassDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name) || 
                string.IsNullOrWhiteSpace(dto.SchoolLevel) || 
                string.IsNullOrWhiteSpace(dto.AcademicYear))
                return BadRequest(new { Message = "Name, SchoolLevel and AcademicYear are required." });

            var validLevels = new[] { "Primary", "Secondary" };
            if (!validLevels.Contains(dto.SchoolLevel))
                return BadRequest(new { Message = "SchoolLevel must be either 'Primary' or 'Secondary'." });

            var result = await _classService.CreateAsync(dto);
            if (result == null)
                return NotFound(new { message = "Teacher Not found." });

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, UpdateClassDTO dto)
        {
            var result  = await _classService.UpdateAsync(id, dto);
            if (result == null) 
                return NotFound(new { Message = "Class or teacher not found" });
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _classService.DeleteAsync(id);
            if (!success) 
                return BadRequest(new { Message = "Class not found or still has active students ." });
            return Ok(new { Message = "Class deleted successfully" });
        }
    }
}