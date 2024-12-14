using System.ComponentModel.DataAnnotations;

namespace SCMS_back_end.Models.Dto.Response.Assignment
{
    public class DtoStudentAssignmentResponse
    {
        [Key]
        public int AssignmentId { get; set; }
        public string AssignmentName { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public int Mark {  get; set; }
        public DtoStudentAssignmentDetails? StudentAssignment { get; set; }
    }
}
