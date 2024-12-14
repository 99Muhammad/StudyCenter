namespace SCMS_back_end.Models.Dto
{
    public class SavedScoreDto
    {
        public int CorrectAnswersCount { get; set; }
        public int TotalQuestionsCount { get; set; }
        public int QuizMark { get; set; }
        public int Score { get; set; }
        public IEnumerable<StudentAnswer> StudentAnswers { get; set; }
    }
}
