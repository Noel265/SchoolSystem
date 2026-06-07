namespace SchoolSystem.API.DTOs
{
    public class CreateFeeDTO
    {
        public int StudentId { get; set; }
        public string FeeType { get; set; } = string.Empty;
        public decimal AmountDue { get; set; }
        public string Term { get; set; } = string.Empty;
        public string AcademicYear { get; set; } = string.Empty;
        public DateOnly DueDate { get; set; }
    }

    public class RecordPaymentDTO
    {
        public int FeeRecordId { get; set; }
        public decimal AmountPaid { get; set; }
        public string? Notes { get; set; }
    }

    public class FeeResponseDTO
    {
        public int Id { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string AdmissionNumber { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string FeeType { get; set; } = string.Empty;
        public decimal AmountDue { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal Balance => AmountDue - AmountPaid;
        public string PaymentStatus => IsPaid ? "Paid" : "Unpaid";
        public string Term { get; set; } = string.Empty;
        public string AcademicYear { get; set; } = string.Empty;
        public DateOnly DueDate { get; set; }
        public DateOnly? PaymentDate { get; set; }
        public bool IsPaid => AmountPaid >= AmountDue;
    }

    public class FeeSummaryDTO
    {
        public string StudentName { get; set; } = string.Empty;
        public string AdmissionNumber { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public decimal TotalDue { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal TotalBalance => TotalDue - TotalPaid;
        public List<FeeResponseDTO> Records { get; set; } = new();
    }
}