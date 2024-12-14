using System.Text.Json.Serialization;

namespace SCMS_back_end.Models
{
    public class StudentAnswerResult
    {
        public int Id { get; set; }
        public int StudentAnswerId { get; set; }
        public int QuizResultId { get; set; } // Add this property
        public bool IsCorrect { get; set; }

        [JsonIgnore]
        public virtual StudentAnswer StudentAnswer { get; set; } // Relationship to StudentAnswer

        [JsonIgnore]
        public virtual QuizResult QuizResult { get; set; } // Relationship to QuizResult
    }
}
