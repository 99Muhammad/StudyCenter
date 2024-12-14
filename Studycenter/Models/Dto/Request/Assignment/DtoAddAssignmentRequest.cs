using System.ComponentModel.DataAnnotations;

namespace SCMS_back_end.Models.Dto.Request.Assignment
{
    public class DtoAddAssignmentRequest
    {
        //[Key]
       // public int AssignmentId { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        [MaxLength(255)]
        public string AssignmentName { get; set; } = string.Empty;

        public DateTime DueDate { get; set; }

        public string Description { get; set; } = string.Empty;

        public bool Visible { get; set; }

        public int FullMark { get; set; }
    }
}
