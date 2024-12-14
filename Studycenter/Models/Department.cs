using System.ComponentModel.DataAnnotations;

namespace SCMS_back_end.Models
{
    public class Department
    {
        [Key]
        public int DepartmentId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        public ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();
        public ICollection<Subject> Subjects { get; set; } = new List<Subject>();
    }

}
