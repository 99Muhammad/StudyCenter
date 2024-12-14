using SCMS_back_end.Models.Dto.Response.Assignment;
using System.ComponentModel.DataAnnotations;

namespace SCMS_back_end.Models
{
    public class Assignment
    {
        [Key]
        public int AssignmentId { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        [MaxLength(255)]
        public string AssignmentName { get; set; } = string.Empty;

        public DateTime DueDate { get; set; }

        public string Description { get; set; }= string.Empty;
        public bool Visible { get; set; }
        //public string? FilePath { get; set; } = string.Empty;
        public int FullMark { get; set; }
        // Navigation property
        public Course Course { get; set; }

        public ICollection<StudentAssignment> StudentAssignments { get; set; }= new List<StudentAssignment>();
    

    }

}
