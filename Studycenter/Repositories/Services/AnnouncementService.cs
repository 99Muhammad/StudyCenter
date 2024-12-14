using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SCMS_back_end.Data;
using SCMS_back_end.Models;
using SCMS_back_end.Models.Dto.Request;
using SCMS_back_end.Models.Dto.Request.Announcement;
using SCMS_back_end.Models.Dto.Response;
using SCMS_back_end.Models.Dto.Response.Announcement;
using SCMS_back_end.Models.Dto.Response.Assignment;
using SCMS_back_end.Repositories.Interfaces;
using SendGrid.Helpers.Mail;
using System.Security.Claims;

namespace SCMS_back_end.Repositories.Services
{
    public class AnnouncementService : IAnnouncement
    {
        private readonly StudyCenterDbContext _context;
     
        public AnnouncementService(StudyCenterDbContext context)
        {
            _context = context;
        }

        public async Task<List<DtoGetAnnouncementRes>> GetAllStudentAnnouncement()
        {
            var announcements = await _context.Announcements
               .Include(a => a.Audience)
               .Where(a => a.Audience.Name == "Students")
               .Select(a => new DtoGetAnnouncementRes
               {
                   AnnouncementId = a.AnnouncementId,
                   Title = a.Title,
                   Content = a.Content,
                   CreatedAt = a.CreatedAt,
                   Type = a.Type,
               }).OrderByDescending(a => a.CreatedAt).ToListAsync();

            return announcements;
        }

        public async Task<List<DtoGetAnnouncementRes>> GetAllTeacherAnnouncement()
        {
            
            var announcements = await _context.Announcements
                .Include (a => a.Audience)
                .Where(a => a.Audience.Name == "Teachers")  
                .Select(a => new DtoGetAnnouncementRes
                {
                    AnnouncementId = a.AnnouncementId,
                    Title = a.Title,
                    Content = a.Content,
                    CreatedAt = a.CreatedAt,
                    Type = a.Type,
                }).OrderByDescending(a => a.CreatedAt).ToListAsync();

            return announcements;
        }

        public async Task<List<DtoGetAnnouncementRes>> GetAnnouncementByCourseID(int courseId)
        {
            var Announcement = await _context.CourseAnnouncements
                 .Where(x => x.CourseId == courseId)
                 .Include(x => x.Announcement)
                 .ThenInclude(a=>a.User)
                 .ThenInclude(u=>u.Teacher)
                 .Select(a => new DtoGetAnnouncementRes
                 {
                     AnnouncementId= a.AnnouncementId,
                     Title = a.Announcement.Title,
                     Content = a.Announcement.Content,
                     CreatedAt = a.Announcement.CreatedAt,
                     UserId = a.Announcement.UserId,
                     UserName = a.Announcement.User.Teacher != null ? a.Announcement.User.Teacher.FullName : "N/A",
                     Type = a.Announcement.Type
                 }).OrderByDescending(a => a.CreatedAt).ToListAsync();
        
            return Announcement;
        }

        public async Task<object> PostAnnouncementByAdmin(DtoPostAnnouncementByAdmin Announcement, ClaimsPrincipal userPrincipal)
        {
            var userIdClaim = userPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var audience = await _context.Audiences.FirstOrDefaultAsync(a => a.Name == Announcement.Audience);
            if (audience == null)
            {
                throw new ArgumentException("Audience can be either Teachers or Students");
            }
            var NewAnnouncement = new Announcement()
            {               
                UserId = userIdClaim,
                Title = Announcement.Title,
                Content = Announcement.Content,
                CreatedAt = DateTime.Now,
                AudienceId = audience.AudienceId,
                Type = Announcement.Type
            };

            _context.Announcements.Add(NewAnnouncement);
            await _context.SaveChangesAsync();

            return new
            {
                Title= NewAnnouncement.Title,
                Content= NewAnnouncement.Content,
                CreatedAt= NewAnnouncement.CreatedAt,
                Audience= NewAnnouncement.Audience.Name,
                Type = NewAnnouncement.Type
            };
        }

        public async Task<object> PostAnnouncementByTeacher(DtoPostAnnouncementByTeacher Announcement, int courseId, ClaimsPrincipal userPrincipal)
        {
            var userIdClaim = userPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var teacher = await _context.Teachers
                .Include(t => t.Courses)
                .FirstOrDefaultAsync(s => s.UserId == userIdClaim);
            if (teacher == null)
            {
                throw new InvalidOperationException("Teacher not found.");
            }
            var course = teacher.Courses.FirstOrDefault(t => t.CourseId == courseId);          
            if (course == null)
            {
                throw new ArgumentException("Invalid Course ID", nameof(courseId));               
            }
            var audience = await _context.Audiences.FirstOrDefaultAsync(a => a.Name == "Course");
            //if (audience == null)
            //{
            //    throw new ArgumentException("Audience can be either Teachers or Students");
            //}


            var newAnnouncement = new Announcement
            {
                Title = Announcement.Title,
                Content =Announcement.Content,
                CreatedAt = DateTime.Now,
                UserId = teacher.UserId,  
                AudienceId = audience.AudienceId,
                Type = Announcement.Type
            };
            
            _context.Announcements.Add(newAnnouncement);
            await _context.SaveChangesAsync();

            
            var newCourseAnnouncement = new CourseAnnouncement
            {
                CourseId = course.CourseId,
                AnnouncementId = newAnnouncement.AnnouncementId
            };

           
            _context.CourseAnnouncements.Add(newCourseAnnouncement);
            await _context.SaveChangesAsync();

            return new
            {
                Title= newAnnouncement.Title,
                Content=newAnnouncement.Content,
                CreatedAt= newAnnouncement.CreatedAt,
                TeacherId= teacher.TeacherId,
                CourseId= course.CourseId,
                Type = newAnnouncement.Type
            };
        }


        public async Task<object> UpdateAnnouncementAsync(int id, DtoPostAnnouncementByAdmin Announcement, ClaimsPrincipal userPrincipal)
        {
            var userIdClaim = userPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var announcement = await _context.Announcements.FindAsync(id);
            if (announcement != null && announcement.UserId==userIdClaim)
            {
                announcement.Title = Announcement.Title;
                announcement.Content = Announcement.Content;
                announcement.Type = Announcement.Type;
                await _context.SaveChangesAsync();

                return new 
                {
                    Title= announcement.Title,
                    Content= announcement.Content,
                    Type= announcement.Type
                };
            }
            return null;
        }
        public async Task<bool> DeleteAnnouncementAsync(int id, ClaimsPrincipal userPrincipal)
        {
            var userIdClaim = userPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var announcement = await _context.Announcements.FindAsync(id);
            if (announcement != null && announcement.UserId == userIdClaim)
            {
                _context.Announcements.Remove(announcement);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

    }
}
