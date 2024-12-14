using System.ComponentModel.DataAnnotations;

namespace SCMS_back_end.Models.Dto.Request.Assignment
{
    public class DtoUpdateAssignmentRequest
    {
        public string AssignmentName { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool Visible { get; set; }
        public int FullMark { get; set; }
    }
}
