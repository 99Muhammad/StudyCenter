namespace SCMS_back_end.Models.Dto.Response.LectureAttendance
{
    public class DtoGetAbsenceRateAndStudentResponse
    {
        public int StudentID { get; set; }
        public string FullName { get; set; } = string.Empty;
        public double AbsenceRateForTheStudent { get; set; }

    }
}
