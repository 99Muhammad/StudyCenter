using System.ComponentModel.DataAnnotations;

namespace SCMS_back_end.Models
{
    public class Subject
    {
        [Key]
        public int SubjectId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(255)]
        public int DepartmentId { get; set; }

        public Department Department { get; set; }  // Navigation property
        public ICollection<Course> Courses { get; set; }= new List<Course>();
    }

}
