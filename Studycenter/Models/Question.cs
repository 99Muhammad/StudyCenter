using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SCMS_back_end.Models
{
    public class Question
    {
        [Key]
        //[JsonIgnore]
        public int QuestionId { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Text { get; set; } = string.Empty;

        [Required]
        public int QuizId { get; set; }

        // Navigation properties

        [JsonIgnore] // Ignore the navigation property during model binding
        public Quiz? Quiz { get; set; }

        //[JsonIgnore]
        public ICollection<AnswerOption>? AnswerOptions { get; set; } = new List<AnswerOption>();
        [JsonIgnore]
        public ICollection<StudentAnswer>? StudentAnswers { get; set; } // Track student answers for this question

    }
}
