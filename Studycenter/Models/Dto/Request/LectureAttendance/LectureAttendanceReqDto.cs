using System.ComponentModel.DataAnnotations;

namespace SCMS_back_end.Models.Dto.Request.LectureAttendance
{
    public class LectureAttendanceReqDto
    {
       //public int LectureId { get; set; }

        public int StudentId { get; set; }

        public string Status { get; set; } = string.Empty;

    }
}
