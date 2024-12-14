using Microsoft.AspNetCore.Mvc;
using SCMS_back_end.Models;
using SCMS_back_end.Models.Dto;
using SCMS_back_end.Models.Dto.Response;
using SCMS_back_end.Models.Dto.Request;
using SCMS_back_end.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace SCMS_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class SubjectController : ControllerBase
    {
        private readonly ISubject _subject;

        public SubjectController(ISubject subject)
        {
            _subject = subject;
        }

        // GET: api/Subject
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Subject>>> GetAllSubjects()
        {
            var subjects = await _subject.GetAllSubjectsAsync();
            return Ok(subjects);
        }

        // GET: api/Subject/5
        [Authorize(Roles= "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Subject>> GetSubjectById(int id)
        {
            var subject = await _subject.GetSubjectByIdAsync(id);
            if (subject == null) return NotFound();
            return Ok(subject);
        }

        // POST: api/Subject
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<DtoSubjectResponse>> AddSubject(DtoSubjectRequest subjectDto)
        {
            if (subjectDto == null)
            {
                return BadRequest("Subject data is required.");
            }
            var createdSubjectDto = await _subject.AddSubjectAsync(subjectDto);
            return CreatedAtAction(nameof(GetSubjectById), new { id = createdSubjectDto.SubjectId }, createdSubjectDto);
        }

        // PUT: api/Subject/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<DtoSubjectResponse>> UpdateSubject(int id, DtoSubjectRequest subjectDto)
        {
            if (subjectDto == null)
            {
                return BadRequest("Subject data is required.");
            }
            var updatedSubjectDto = await _subject.UpdateSubjectAsync(id, subjectDto);
            if (updatedSubjectDto == null) return NotFound();
            return Ok(updatedSubjectDto);
        }

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteSubject(int id)
        //{
        //    var subject = await _subject.GetSubjectByIdAsync(id);
        //    if (subject == null) return NotFound();

        //    var result= await _subject.DeleteSubjectAsync(id);
        //    if(!result) return Conflict(new { Message = "Subject is assigned to active courses" });
        //    return NoContent();
        //}
    }
}
