namespace SCMS_back_end.Models.Dto.Request
{
    public class UpdateStudentAnswerRequestDto
    {
        public int Id { get; set; } 
        public int QuizId { get; set; } // FK to Quiz
        public int QuestionId { get; set; } // FK to Question
        public int SelectedAnswerOptionId { get; set; } // FK to AnswerOption (The selected answer)
        public int StudentId { get; set; } // FK to Student
        public DateTime SubmittedAt { get; set; } // Timestamp for when the answer was submitted
    }
}
