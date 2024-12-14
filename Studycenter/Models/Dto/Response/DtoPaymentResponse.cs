namespace SCMS_back_end.Models.Dto.Response
{
    public class DtoPaymentResponse
    {
        public int PaymentId { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public DateTimeOffset Date { get; set; }
        public decimal Amount { get; set; }
        public string Method { get; set; }
        public string Status { get; set; }
        public string ReferenceNumber { get; set; }
    }
}

