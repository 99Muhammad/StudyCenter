using System.Text.Json.Serialization;

namespace SCMS_back_end.Models
{
    public class QuizResult
    {
        public int Id { get; set; }
        public int QuizId { get; set; }          // FK to Quiz
        public int StudentId { get; set; }       // FK to Student (or User)
        public int Score { get; set; }           // The student's score (e.g., number of correct answers)
        public int NumbersOfCorrectAnswers { get; set; }  // The total number of questions in the quiz
        public DateTime SubmittedAt { get; set; }

        // Navigation Properties
        [JsonIgnore]
        public Quiz? Quiz { get; set; }

        [JsonIgnore]
        public Student? Student { get; set; }

        public ICollection<StudentAnswerResult> Answers { get; set; } = new List<StudentAnswerResult>();
    }

}
