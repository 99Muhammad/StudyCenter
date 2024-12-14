namespace SCMS_back_end.Models.Dto
{
    public class QuizResultRequestDto
    {
        public int QuizId { get; set; }          // FK to Quiz
        public int StudentId { get; set; }       // FK to Student (or User)
        public int Score { get; set; }           // The student's score
        public int TotalQuestions { get; set; }  // The total number of questions in the quiz
        public DateTime SubmittedAt { get; set; } // Time when the quiz was submitted
    }
}
