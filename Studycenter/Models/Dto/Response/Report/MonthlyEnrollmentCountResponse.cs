namespace SCMS_back_end.Models.Dto.Response.Report
{
    public class MonthlyEnrollmentCountResponse
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int EnrollmentCount { get; set; }
    }
}
