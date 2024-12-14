namespace SCMS_back_end.Models.Dto.Request.Assignment
{
    public class DtoGradeNotSubmitted
    {
        public int assignmentId { get; set; }
        public int studentId { get; set; }
        public int grade {  get; set; }
        public string feedback { get; set; }
    }
}
