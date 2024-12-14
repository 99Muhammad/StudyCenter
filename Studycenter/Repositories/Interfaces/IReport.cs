using SCMS_back_end.Models.Dto.Response.Report;

namespace SCMS_back_end.Repositories.Interfaces
{
    public interface IReport
    {
        Task<StudentEnrollmentOverview> GetStudentEnrollmentOverview();
        Task<CoursePerformanceOverview> GetCoursePerformanceOverview();
        Task<InstructorEffectivenessOverview> GetInstructorEffectivenessOverview();
        Task GetAssignmentAndAttendanceOverview();
        Task<DepartmentalActivityOverview> GetDepartmentalActivityOverview();
        Task<SystemHealthCheckOverview> GetSystemHealthCheckOverview();
        Task<List<CourseEnrollmentCountResponse>> GetCourseEnrollmentCounts();
        Task<List<MonthlyEnrollmentCountResponse>> GetMonthlyEnrollmentCounts();
        Task<List<CourseCapacityUtilizationResponse>> GetCourseCapacityUtilization();
        Task<List<TeacherCourseLoadResponse>> GetTeacherCourseLoad();
        Task<List<MonthlyRevenueResponse>> GetMonthlyRevenueTrends();
        Task<List<CourseRevenueResponse>> GetCourseRevenue();

    }
}
