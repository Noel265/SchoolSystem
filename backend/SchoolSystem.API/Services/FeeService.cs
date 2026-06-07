using Microsoft.EntityFrameworkCore;
using SchoolSystem.API.Data;
using SchoolSystem.API.DTOs;
using SchoolSystem.API.Models;

namespace SchoolSystem.API.Services
{
    public class FeeService
    {
        private readonly AppDbContext _context;

        public FeeService(AppDbContext context)
        {
            _context = context;
        }

        // Methods for creating fees, recording payments, and retrieving fee information would go here

        public async Task<List<FeeResponseDTO>> GetAllAsync(string? term, string? academicYear)
        {
            var query = _context.FeeRecords
                .Include(fr => fr.Student)
                    .ThenInclude(s => s.Class)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(term))
            {
                query = query.Where(fr => fr.Term == term);
            }

            if (!string.IsNullOrWhiteSpace(academicYear))
            {
                query = query.Where(fr => fr.AcademicYear == academicYear);
            }

            var records = await query.ToListAsync();

            return records.Select(fr => MapToDTO(fr)).ToList();
        }

        public async Task<FeeSummaryDTO?> GetByStudentAsync(int studentId)
        {
            var student = await _context.Students
                .Include(s => s.Class)
                .FirstOrDefaultAsync(s => s.Id == studentId);

            if (student == null) return null;

            var feeRecords = await _context.FeeRecords
                .Include(fr => fr.Student)
                    .ThenInclude(s => s.Class)
                .Where(fr => fr.StudentId == studentId)
                .ToListAsync();

            var summary = new FeeSummaryDTO
            {
                StudentName = student.FullName,
                AdmissionNumber = student.AdmissionNumber,
                ClassName = student.Class != null ? student.Class.Name : string.Empty,
                TotalDue = feeRecords.Sum(fr => fr.AmountDue),
                TotalPaid = feeRecords.Sum(fr => fr.AmountPaid),
                Records = feeRecords.Select(fr => MapToDTO(fr)).ToList()
            };

            return summary;
        }

        public async Task<FeeResponseDTO?> CreateAsync(CreateFeeDTO dto)
        {
            // Validate student exists
            var studentExists = await _context.Students
                .Include(s => s.Class)
                .AnyAsync(s => s.Id == dto.StudentId);
            if (!studentExists) return null;

            var newFeeRecord = new FeeRecord
            {
                StudentId = dto.StudentId,
                FeeType = dto.FeeType,
                AmountDue = dto.AmountDue,
                AmountPaid = 0,
                Term = dto.Term,
                AcademicYear = dto.AcademicYear,
                DueDate = dto.DueDate,
                Status = "Unpaid"
            };

            _context.FeeRecords.Add(newFeeRecord);
            await _context.SaveChangesAsync();

            var createdRecord = await _context.FeeRecords
                .Include(fr => fr.Student)
                    .ThenInclude(s => s.Class)
                .FirstOrDefaultAsync(fr => fr.Id == newFeeRecord.Id);

            if (createdRecord == null)
                {
                    throw new Exception("Failed to retrieve the created fee record.");
                }

            return MapToDTO(createdRecord);
        }

        public async Task<FeeResponseDTO?> RecordPaymentAsync(RecordPaymentDTO dto)
        {
            var feeRecord = await _context.FeeRecords
                .Include(fr => fr.Student)
                    .ThenInclude(s => s.Class)
                .FirstOrDefaultAsync(fr => fr.Id == dto.FeeRecordId);

            if (feeRecord == null) return null;

            feeRecord.AmountPaid += dto.AmountPaid;
            feeRecord.PaymentDate = DateOnly.FromDateTime(DateTime.UtcNow);
            //feeRecord.Notes = dto.Notes;

            if (feeRecord.AmountPaid >= feeRecord.AmountDue)
                feeRecord.Status = "Paid";
            else if (feeRecord.AmountPaid > 0)
                feeRecord.Status = "Partially Paid";
            else
                feeRecord.Status = "Unpaid";            

            await _context.SaveChangesAsync();

            return MapToDTO(feeRecord);
        }

        public async Task<List<FeeResponseDTO>> GetOverDueAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var overdueRecords = await _context.FeeRecords
                .Include(fr => fr.Student)
                    .ThenInclude(s => s.Class)
                .Where(fr => fr.DueDate < today && fr.Status != "Paid")
                .ToListAsync();

            return overdueRecords.Select(fr => MapToDTO(fr)).ToList();
        }

        private static FeeResponseDTO MapToDTO(FeeRecord fr)
        {
            return new FeeResponseDTO
            {
                Id = fr.Id,
                StudentName = fr.Student.FullName,
                AdmissionNumber = fr.Student.AdmissionNumber,
                ClassName = fr.Student.Class != null ? fr.Student.Class.Name : string.Empty,
                FeeType = fr.FeeType,
                AmountDue = fr.AmountDue,
                AmountPaid = fr.AmountPaid,
                //Balance = fr.AmountDue - fr.AmountPaid,
                //PaymentStatus = fr.Status,
                Term = fr.Term,
                AcademicYear = fr.AcademicYear,
                DueDate = fr.DueDate,
                PaymentDate = fr.PaymentDate
            };
        }
    }
}