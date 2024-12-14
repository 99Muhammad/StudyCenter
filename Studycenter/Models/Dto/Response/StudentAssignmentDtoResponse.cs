namespace SCMS_back_end.Models.Dto.Response
{
    public class StudentAssignmentDtoResponse
    {
        public int StudentAssignmentId { get; set; } // Unique ID of the submission
        public int AssignmentId { get; set; }
        public int StudentId { get; set; }
        public DateTime? SubmissionDate { get; set; } // Made nullable
        public string Submission { get; set; } = string.Empty;
        public int? Grade { get; set; }
        public string Feedback { get; set; } = string.Empty;
        public string? FilePath { get; set; } // Path to the submitted file (if any)
    }
}
