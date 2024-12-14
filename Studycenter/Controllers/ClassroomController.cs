using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SCMS_back_end.Models.Dto.Request;
using SCMS_back_end.Models.Dto.Response;
using SCMS_back_end.Repositories.Interfaces;

namespace SCMS_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassroomController : ControllerBase
    {
        private readonly IClassroom _classroomService;
        public ClassroomController(IClassroom context)
        {
            _classroomService = context;
        }

        // GET: api/Classroom
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<DtoClassroomResponse>>> GetAllClassrooms()
        {
            var classroom = await _classroomService.GetAllClassroomsAsync();
            return Ok(classroom);
        }

        // GET: api/Classroom/5
        [HttpGet("{id}")]
        [Authorize(Roles= "Admin")]
        public async Task<ActionResult<DtoClassroomResponse>> GetClassroom(int id)
        {
            var classroom = await _classroomService.GetClassroomByIdAsync(id);
            if (classroom == null) return NotFound();
            return Ok(classroom);
        }

        // PUT: api/Classroom/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DtoClassroomResponse>> UpdateClassroom(int id, DtoUpdateClassroomRequest classroomRequest)
        {
            if (classroomRequest == null)
            {
                return BadRequest("Classroom Name is required.");
            }
            var updatedClassroom = await _classroomService.UpdateClassroomAsync(id, classroomRequest);
            if (updatedClassroom == null) return NotFound();
            return Ok(updatedClassroom);
        }

        // POST: api/Classroom
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DtoClassroomResponse>> AddClassroom(DtoCreateClassroomRequest classroomRequest)
        {
            if (classroomRequest == null)
            {
                return BadRequest("Classroom Name is required.");
            }
            var classroom = await _classroomService.AddClassroomAsync(classroomRequest);
            return CreatedAtAction(nameof(GetClassroom), new { id = classroom.ClassroomId }, classroom);
        }

        // DELETE: api/Classroom/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteClassroom(int id)
        {
            var classroom = await _classroomService.GetClassroomByIdAsync(id);
            if (classroom == null) return NotFound();

            var result = await _classroomService.DeleteClassroomAsync(id);
            if (!result) return Conflict(new { Message = "Classroom has subjects with active courses" });
            return NoContent();
        }
    }
}
