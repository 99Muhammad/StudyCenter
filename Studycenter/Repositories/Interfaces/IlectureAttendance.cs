using SCMS_back_end.Models;
using SCMS_back_end.Models.Dto.Request.LectureAttendance;
using SCMS_back_end.Models.Dto.Response.LectureAttendance;
using System.Security.Claims;

namespace SCMS_back_end.Repositories.Interfaces
{
    public interface ILectureAttendance
    {
        public Task<List<LectureAttendance>> AddLectureAttendance(AddLectureAttendanceReqDto Attendance);
        public Task<List<DtoGetAbsenceDateResponse>> GetAbsenceDate(int CourseId, ClaimsPrincipal userPrincipal);
        public Task<List<DtoGetAbsenceRateAndStudentResponse>> GetStudentAndAbsenceRate(int CourseId);
        public Task<List<Lecture>> GetCourseLectures(int courseId);
        public Task<List<DtoLectursResponse>> GetLectures(int CourseId);

    }
}
