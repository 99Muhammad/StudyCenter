using System.ComponentModel.DataAnnotations;

namespace SCMS_back_end.Models
{
    public class Teacher
    {
        [Key]
        public int TeacherId { get; set; }
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string FullName { get; set; } = string.Empty;

        public int CourseLoad { get; set; }

        public string PhoneNumber { get; set; } = string.Empty;

        public int DepartmentId { get; set; }

        public Department Department { get; set; } // Navigation property
        public User User { get; set; }  // Navigation property
        public ICollection<Course> Courses { get; set; } = new List<Course>();

    }
}
