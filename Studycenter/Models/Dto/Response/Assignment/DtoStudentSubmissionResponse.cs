namespace SCMS_back_end.Models.Dto.Response.Assignment
{
    public class DtoStudentSubmissionResponse
    {
        public int StudentId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public DtoStudentAssignmentDetails? StudentAssignment { get; set; }
    }
}
