using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SCMS_back_end.Models;
using SCMS_back_end.Models.Dto.Request.Announcement;
using SCMS_back_end.Models.Dto.Request.Assignment;
using SCMS_back_end.Models.Dto.Response;
using SCMS_back_end.Models.Dto.Response.Announcement;
using SCMS_back_end.Models.Dto.Response.Assignment;
using SCMS_back_end.Repositories.Interfaces;

namespace SCMS_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnnouncementController : ControllerBase
    {
        private readonly IAnnouncement _Announcement;
        public AnnouncementController(IAnnouncement context)
        {
            _Announcement = context;
        }
        // POST: api/Announcement
        [Authorize(Roles ="Admin")]
        [HttpPost]
        public async Task<IActionResult> PostAnnouncementByAdmin(DtoPostAnnouncementByAdmin Announcement)
        {
            try
            {
                var response = await _Announcement.PostAnnouncementByAdmin(Announcement, User);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        // POST: api/Announcement
        [Authorize(Roles ="Teacher")]
        [HttpPost("Course/{courseId}")]
        public async Task<IActionResult> PostAnnouncementByTeacher(DtoPostAnnouncementByTeacher Announcement, int courseId)
        {
            try
            {
                var response = await _Announcement.PostAnnouncementByTeacher(Announcement, courseId, User);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        [Authorize(Roles = "Teacher, Admin")]
        [HttpGet("Teachers")]
        public async Task<ActionResult<List<DtoGetAnnouncementRes>>> GetTeachersAnnouncement()
        {
            try
            {
                var AllAnnouncement = await _Announcement.GetAllTeacherAnnouncement();
                if (AllAnnouncement == null)
                    return NotFound("No Announcement found .");

                return Ok(AllAnnouncement);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }


        }

        [Authorize(Roles = "Student, Admin")]
        [HttpGet("Students")]
        public async Task<ActionResult<List<DtoGetAnnouncementRes>>> GetStudentsAnnouncement()
        {
            try
            {
                var AllAnnouncement = await _Announcement.GetAllStudentAnnouncement();
                if (AllAnnouncement == null)
                    return NotFound("No Announcement found .");

                return Ok(AllAnnouncement);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }


        }

        [Authorize(Roles = "Student, Teacher")]
        // GET: api/Announcement/id
        [HttpGet("Course/{courseId}")]
        public async Task<ActionResult<DtoGetAnnouncementRes>> GetCourseAnnouncement(int courseId)
        {
            var announcement = await _Announcement.GetAnnouncementByCourseID(courseId);

            if (announcement == null)
            {
                return NotFound();
            }

            return Ok(announcement);
        }

        [Authorize(Roles ="Admin, Teacher")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAnnouncement(int id)
        {
            var result= await _Announcement.DeleteAnnouncementAsync(id, User);
            if (!result) return NotFound();
            return NoContent();
        }

        [Authorize(Roles = "Admin, Teacher")]
        [HttpPut("{id}")]
        public async Task<ActionResult<DtoSubjectResponse>> UpdateAnnouncement(int id, DtoPostAnnouncementByAdmin announcementDto)
        {
            if (announcementDto == null)
            {
                return BadRequest("Announcement data is required.");
            }
            var result = await _Announcement.UpdateAnnouncementAsync(id, announcementDto, User);
            if (result == null) return NotFound();
            return Ok(result);
        }
    }
}
