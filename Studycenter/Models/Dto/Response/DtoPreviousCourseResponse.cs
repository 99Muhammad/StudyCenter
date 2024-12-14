namespace SCMS_back_end.Models.Dto.Response
{
    public class DtoPreviousCourseResponse
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public string TeacherName { get; set; }
        public string SubjectName { get; set; }
        public double Grade { get; set; }
        //public int Level { get; set; }
        public string Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
