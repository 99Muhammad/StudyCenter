using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SCMS_back_end.Models
{
    public class Quiz
    {
        [Key]
        //[JsonIgnore]
        public int QuizId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; } = string.Empty;

        public int Duration { get; set; } // Duration in minutes
        public int Mark { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsVisible { get; set; } // Indicates if quiz is visible to students
                                            // Foreign key to Course
        public int? CourseId { get; set; } // Make it nullable for now

        [JsonIgnore]
        public Course? Course { get; set; }  // Navigation property

        //[JsonIgnore]
        public ICollection<QuizResult> QuizResult { get; set; }= new List<QuizResult>();

        // Navigation properties
        //[JsonIgnore]
        public ICollection<Question> Questions { get; set; } = new List<Question>();
        [JsonIgnore]
        public ICollection<StudentQuiz> StudentQuizzes { get; set; } = new List<StudentQuiz>();

        // Navigation Properties
        [JsonIgnore]
        public ICollection<StudentAnswer> StudentAnswers { get; set; } = new List<StudentAnswer>(); // New relationship

    }
}
