using System.ComponentModel.DataAnnotations;

namespace SCMS_back_end.Models.Dto.Response.Assignment
{
    public class DtoStudentAssignmentDetails
    {
        [Key]
        public int StudentAssignmentId { get; set; }
        public DateTime? SubmissionDate { get; set; }
        public int? Grade { get; set; }
        public string? Feedback { get; set; }
      
    }
}
