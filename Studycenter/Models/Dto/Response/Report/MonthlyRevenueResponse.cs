namespace SCMS_back_end.Models.Dto.Response.Report
{
    public class MonthlyRevenueResponse
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
