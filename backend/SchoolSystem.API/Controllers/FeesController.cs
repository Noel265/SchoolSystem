using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolSystem.API.DTOs;
using SchoolSystem.API.Services;

namespace SchoolSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FeesController : ControllerBase
    {
        private readonly FeeService _feeService;

        public FeesController(FeeService feeService)
        {
            _feeService = feeService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? term, 
            [FromQuery] string? academicYear)
        {
            var fees = await _feeService.GetAllAsync(term, academicYear);
            return Ok(fees);
        }

        [HttpGet("student/{studentId}")]
        [Authorize(Roles = "Admin,Parent")]
        public async Task<IActionResult> GetByStudent(int studentId)
        {
            var summary = await _feeService.GetByStudentAsync(studentId);
            if (summary == null)
                return NotFound(new { message = "Student not found." });

            return Ok(summary);
        }

        [HttpGet("Overdue")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetOverdue()
        {
            var overdueFees = await _feeService.GetOverDueAsync();
            return Ok(overdueFees);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateFeeDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FeeType) ||
                dto.AmountDue <= 0 || dto.StudentId <= 0 )
            {
                return BadRequest(new { message = "All fields are required." });
            }

            var createdFee = await _feeService.CreateAsync(dto);
            if (createdFee == null)
                return BadRequest(new { message = "Failed to create fee record." });

            return CreatedAtAction(nameof(GetByStudent), new { studentId = dto.StudentId }, createdFee);
        }

        [HttpPut("payment")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RecordPayment( RecordPaymentDTO dto)
        {
            if (dto.AmountPaid <= 0)
                return BadRequest(new { message = "Amount paid must be greater than 0." });

            var updatedFee = await _feeService.RecordPaymentAsync(dto);
            if (updatedFee == null)
                return BadRequest(new { message = "Failed to record payment." });

            return Ok(updatedFee);
        }
    }
}