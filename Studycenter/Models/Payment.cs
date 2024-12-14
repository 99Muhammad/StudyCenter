using System.ComponentModel.DataAnnotations.Schema;

namespace SCMS_back_end.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public DateTime Date { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        public string Method { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string ReferenceNumber { get; set; } = string.Empty;
        public Student Student { get; set; }
        public Course Course { get; set; }
    }
}
