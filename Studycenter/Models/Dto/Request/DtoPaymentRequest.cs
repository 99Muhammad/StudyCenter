using System;

namespace SCMS_back_end.Models.Dto.Request
{
    public class DtoPaymentRequest
    {
        //public int StudentId { get; set; }
        public int CourseId { get; set; }
        //public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Method { get; set; }
        public string Status { get; set; }
        public string ReferenceNumber { get; set; }
    }
}
