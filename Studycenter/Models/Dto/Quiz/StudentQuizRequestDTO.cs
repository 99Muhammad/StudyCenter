namespace SCMS_back_end.Models.Dto.Quiz
{
    public class StudentQuizRequestDTO
    {
        public int QuizId { get; set; }
        public List<StudentAnswerDto> studentAnswers { get; set; } = new List<StudentAnswerDto>();
    }
}
