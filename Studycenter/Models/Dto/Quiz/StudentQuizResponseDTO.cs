namespace SCMS_back_end.Models.Dto.Quiz
{
    public class StudentQuizResponseDTO
    {
        public int QuizId { get; set; }
        public string Title { get; set; }
        public int Duration { get; set; }
        public bool IsVisible { get; set; }
        public int? CourseId { get; set; }
        public int Mark { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public QuizResult? QuizResult { get; set; } // Nullable
    }
}
