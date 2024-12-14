namespace SCMS_back_end.Models.Dto.Request.LectureAttendance
{
    public class AddLectureAttendanceReqDto
    {
        public int LectureId { get; set; }
        public List<LectureAttendanceReqDto> LectureAttendance { get; set; } = new List<LectureAttendanceReqDto>();
    }
}
