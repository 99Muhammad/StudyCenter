using System.ComponentModel.DataAnnotations;

namespace SCMS_back_end.Models.Dto.Response.LectureAttendance
{
    public class DtoAddLectureAttendanceResponse
    {
      
        public int LectureId { get; set; }

        public int StudentId { get; set; }

        public string Status { get; set; } = string.Empty;

        //public Lecture Lecture { get; set; } // Navigation property
        //public Student Student { get; set; }  // Navigation property
    }
}
