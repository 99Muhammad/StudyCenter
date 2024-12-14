namespace SCMS_back_end.Models.Dto.Request
{
    public class QuizUpdateDto
    {
        public string Title { get; set; }
        public int Duration { get; set; }
        public bool IsVisible { get; set; }
        public int CourseId { get; set; }
    }

}
