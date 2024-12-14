using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SCMS_back_end.Models
{
    public class Lecture
    {
        [Key]
        public int LectureId { get; set; }

        [Required]
        public int CourseId { get; set; }

        public DateTime LectureDate { get; set; }

        [JsonIgnore]
        public Course Course { get; set; } // Navigation property
        public ICollection<LectureAttendance> LectureAttendances { get; set; } = new List<LectureAttendance>();
    }

}
