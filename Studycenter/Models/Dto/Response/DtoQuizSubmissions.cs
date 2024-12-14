namespace SCMS_back_end.Models.Dto.Response
{
    public class DtoQuizSubmissions
    {
        public int Id { get; set; }
        public string StudentName { get; set; }
        public DateTime SubmissionDate { get; set; }
        public int Grade { get; set; }
    }
}
