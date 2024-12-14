namespace SCMS_back_end.Models.Dto.Request
{
    public class DtoUpdateCourseRequest
    {
        public int? TeacherId { get; set; }
        public int Capacity { get; set; }
        public int ClassroomId { get; set; }
    }
}
