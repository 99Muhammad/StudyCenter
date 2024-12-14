using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SCMS_back_end.Data;
using SCMS_back_end.Models;
using SCMS_back_end.Models.Dto.Request;
using SCMS_back_end.Models.Dto.Response;
using SCMS_back_end.Repositories.Interfaces;

namespace SCMS_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly StudyCenterDbContext _context;
        private readonly IStudent _studentService;

        public StudentsController(StudyCenterDbContext context, IStudent studentService)
        {
            _context = context;
            _studentService = studentService;
        }

        // GET: api/Students
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentDtoResponse>>> GetStudents()
        {
            try
            {
                var students = await _studentService.GetAllStudentsAsync();

                if (students == null || !students.Any())
                {
                    return NotFound("No students found.");
                }

                //return Ok(students);
                return Ok(new { message = "Request successful", data = students, timestamp = DateTime.Now });

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Students/5
        [Authorize(Roles = "Admin,Teacher,Student")]
        [HttpGet("{id}")]
        public async Task<ActionResult<StudentDtoResponse>> GetStudent(int id)
        {
            try
            {
                var studentDto = await _studentService.GetStudentByIdAsync(id);

                if (studentDto == null)
                {
                    return NotFound();
                }

                return Ok(studentDto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }
        
        
        // PUT: api/Students/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStudent(int id, StudentDtoRequest studentDto)
        {
            if (studentDto == null)
            {
                return BadRequest("Student data is required.");
            }

            try
            {
                await _studentService.UpdateStudentAsync(id, studentDto);
                return Ok("Student updated successfully.");
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Student with ID {id} not found.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/Students/5
        //[Authorize(Roles = "Admin")]
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteStudent(int id)
        //{
        //    try
        //    {
        //        await _studentService.DeleteStudentAsync(id);
        //        return NoContent(); // 204 No Content response
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        return NotFound(new { message = ex.Message }); // 404 Not Found
        //    }
        //    catch (InvalidOperationException ex)
        //    {
        //        return BadRequest(new { message = ex.Message }); // 400 Bad Request
        //    }   
        //}

        // POST: api/Students/drop

        [HttpPost("drop")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DropStudentFromCourse([FromBody] EnrollmentDtoRequest enrollmentDto)
        {
            if (enrollmentDto == null)
            {
                return BadRequest("Enrollment data is required.");
            }

            try
            {
                await _studentService.DropStudentFromCourseAsync(enrollmentDto.StudentId, enrollmentDto.CourseId);
                return Ok("Student dropped from course successfully.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Students/course/5
        [Authorize(Roles = "Teacher,Student")]
        [HttpGet("course/{courseId}")]
        public async Task<ActionResult<IEnumerable<StudentDtoResponse>>> GetStudentsByCourseId(int courseId)
        {
            try
            {
                var students = await _studentService.GetStudentsByCourseIdAsync(courseId);

                if (students == null || !students.Any())
                {
                    return NotFound("No students found for the given course ID.");
                }

                return Ok(students);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/Students/enroll
        [Authorize(Roles = "Student")]
        [HttpPost("enroll/{courseId}")]
        public async Task<IActionResult> EnrollStudentInCourse(int courseId)
        {
            //if (enrollmentDto == null)
            //{
            //    return BadRequest("Enrollment data is required.");
            //}

            try
            {
                await _studentService.EnrollStudentInCourseAsync(User, courseId);
                return Ok("Student enrolled successfully.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        private async Task<bool> StudentExists(int id)
        {
            return await _context.Students.AnyAsync(e => e.StudentId == id);
        }
    }
}
