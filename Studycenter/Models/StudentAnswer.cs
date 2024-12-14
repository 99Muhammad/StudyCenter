namespace SCMS_back_end.Models
{
    using System;
    using System.Text.Json.Serialization;

    public class StudentAnswer
    {
        [JsonIgnore]
        public int? Id { get; set; }
        public int QuizId { get; set; } // FK to Quiz
        public int QuestionId { get; set; } // FK to Question
        public int SelectedAnswerOptionId { get; set; } // FK to AnswerOption (The selected answer)
        public int StudentId { get; set; } // FK to Student (or User)
        public DateTime SubmittedAt { get; set; }

        // Navigation Properties (Nullable and JsonIgnore)
        [JsonIgnore]
        public Quiz? Quiz { get; set; }

        [JsonIgnore]
        public Question? Question { get; set; }

        [JsonIgnore]
        public AnswerOption? SelectedAnswerOption { get; set; }

        [JsonIgnore]
        public Student? Student { get; set; }

        [JsonIgnore]
        public StudentAnswerResult? StudentAnswerResult { get; set; }

    }


}
