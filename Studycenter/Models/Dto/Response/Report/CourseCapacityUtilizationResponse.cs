namespace SCMS_back_end.Models.Dto.Response.Report
{
    public class CourseCapacityUtilizationResponse
    {
        public string CourseName { get; set; }
        public int Capacity { get; set; }
        public int ActualEnrollment { get; set; }
    }
}
