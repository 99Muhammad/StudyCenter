namespace SCMS_back_end.Models.Dto.Request
{
    // DTO for Student Assignment Submission
    public class StudentAssignmentSubmissionDtoRequest
    {
        public int AssignmentId { get; set; }
        //public int StudentId { get; set; }
        //public DateTime? SubmissionDate { get; set; } // Optional
        public string Submission { get; set; } = string.Empty; // Text-based submission (if any)
        public IFormFile? File { get; set; } // File upload
    }
}
