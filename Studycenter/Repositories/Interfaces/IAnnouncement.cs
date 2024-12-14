using SCMS_back_end.Models;
using SCMS_back_end.Models.Dto.Request.Announcement;
using SCMS_back_end.Models.Dto.Response;
using SCMS_back_end.Models.Dto.Response.Announcement;
using SCMS_back_end.Models.Dto.Response.Assignment;
using System.Security.Claims;

namespace SCMS_back_end.Repositories.Interfaces
{
    public interface IAnnouncement
    {
        Task<object> PostAnnouncementByAdmin(DtoPostAnnouncementByAdmin Announcement, ClaimsPrincipal userPrincipal);

        Task<object> PostAnnouncementByTeacher(DtoPostAnnouncementByTeacher Announcement,int CourseId, ClaimsPrincipal userPrincipal);

        Task<List<DtoGetAnnouncementRes>> GetAllTeacherAnnouncement();

        Task<List<DtoGetAnnouncementRes>> GetAllStudentAnnouncement();

        public Task<List<DtoGetAnnouncementRes>> GetAnnouncementByCourseID(int courseId);

        public Task<object> UpdateAnnouncementAsync(int id, DtoPostAnnouncementByAdmin Announcement, ClaimsPrincipal userPrincipal);
        public  Task<bool> DeleteAnnouncementAsync(int id, ClaimsPrincipal userPrincipal);
    }
}
