using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SCMS_back_end.Repositories.Interfaces;
using SCMS_back_end.Repositories.Services;
using System.Threading.Tasks;

namespace SCMS_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="Admin")]
    public class ReportController : ControllerBase
    {
        private readonly IReport _reportService;
        public ReportController(IReport reportService)
        {
            _reportService = reportService;
        }
        [HttpGet("student-enrollment-overview")]
        public async Task<IActionResult> GetStudentEnrollmentOverview()
        {
            var result = await _reportService.GetStudentEnrollmentOverview();
            return Ok(result);
        }

        [HttpGet("course-performance-overview")]
        public async Task<IActionResult> GetCoursePerformanceOverview()
        {
            var result = await _reportService.GetCoursePerformanceOverview();
            return Ok(result);
        }

        [HttpGet("instructor-effectiveness-overview")]
        public async Task<IActionResult> GetInstructorEffectivenessOverview()
        {
            var result = await _reportService.GetInstructorEffectivenessOverview();
            return Ok(result);
        }

        [HttpGet("assignment-and-attendance-overview")]
        public async Task<IActionResult> GetAssignmentAndAttendanceOverview()
        {
            await _reportService.GetAssignmentAndAttendanceOverview();
            return Ok();
        }

        [HttpGet("departmental-activity-overview")]
        public async Task<IActionResult> GetDepartmentalActivityOverview()
        {
            var result = await _reportService.GetDepartmentalActivityOverview();
            return Ok(result);
        }

        [HttpGet("system-health-check-overview")]
        public async Task<IActionResult> GetSystemHealthCheckOverview()
        {
            var result = await _reportService.GetSystemHealthCheckOverview();
            return Ok(result);
        }

        [HttpGet("enrollment-trends")]
        public async Task<IActionResult> GetEnrollmentTrends()
        {
            var result = await _reportService.GetCourseEnrollmentCounts();
            return Ok(result);
        }

        [HttpGet("student-enrollment-trends")]
        public async Task<IActionResult> GetStudentEnrollmentTrends()
        {
            var result = await _reportService.GetMonthlyEnrollmentCounts();
            return Ok(result);
        }

        [HttpGet("course-capacity-utilization")]
        public async Task<IActionResult> GetCourseCapacitiUtilization()
        {
            var result = await _reportService.GetCourseCapacityUtilization();
            return Ok(result);
        }

        [HttpGet("teacher-course-loads")]
        public async Task<IActionResult> GetTeacherCourseLoads()
        {
            var result = await _reportService.GetTeacherCourseLoad();
            return Ok(result);
        }

        [HttpGet("monthly-revenue-trends")]
        public async Task<IActionResult> GetMonthlyRevenueTrends()
        {
            var result = await _reportService.GetMonthlyRevenueTrends();
            return Ok(result);
        }

        [HttpGet("course-revenue")]
        public async Task<IActionResult> GetCourseRevenue()
        {
            var result = await _reportService.GetCourseRevenue();
            return Ok(result);
        }
    }
}
