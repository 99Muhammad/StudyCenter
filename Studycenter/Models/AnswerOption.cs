using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SCMS_back_end.Models
{
    public class AnswerOption
    {
        [Key]
        //[JsonIgnore]
        public int AnswerOptionId { get; set; }

        [Required]
        public string Text { get; set; } = string.Empty;

        public bool IsCorrect { get; set; } // Indicates if the answer is correct

        [Required]
        public int QuestionId { get; set; }

        // Navigation property
        [JsonIgnore]
        public Question? Question { get; set; }
        [JsonIgnore]
        public ICollection<StudentAnswer>? StudentAnswers { get; set; }
    }
}
