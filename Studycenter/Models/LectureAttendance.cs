using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SCMS_back_end.Models
{
    public class LectureAttendance
    {
        [Key]
        [JsonIgnore]
        public int LectureAttendanceId { get; set; }

        [Required]
        public int LectureId { get; set; }

        [Required]
        public int StudentId { get; set; }

        [MaxLength(255)]
        public string Status { get; set; } = string.Empty;
        [JsonIgnore]
        public Lecture Lecture { get; set; } // Navigation property
        [JsonIgnore]
        public Student Student { get; set; }  // Navigation property
    }

}
