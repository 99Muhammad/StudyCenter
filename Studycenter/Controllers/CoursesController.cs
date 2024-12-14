using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SCMS_back_end.Data;
using SCMS_back_end.Models;
using SCMS_back_end.Repositories.Interfaces;
using SCMS_back_end.Models.Dto.Request;
using SCMS_back_end.Models.Dto.Response;
using SCMS_back_end.Repositories.Services;
using Microsoft.AspNetCore.Authorization;
using SCMS_back_end.Models.Dto.Grades;


namespace SCMS_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Teacher, Student")]

    public class CoursesController : ControllerBase
    {
        private readonly ICourse _course;
        public CoursesController(ICourse course)
        {
            _course = course;
        }

        // POST: api/Courses
        [Authorize(Roles ="Admin")]
        [HttpPost]
        public async Task<ActionResult<Course>> PostCourse(DtoCreateCourseWTRequest course)
        {
            try 
            {
                var newCourse = await _course.CreateCourseWithoutTeacher(course);
                return Ok(newCourse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }

        }
        
        // Tested
        // PUT: api/Courses/5
        [Authorize(Roles ="Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<Course>> PutCourse(int id, DtoUpdateCourseRequest course)
        {
            try
            {
                var updatedCourse = await _course.UpdateCourseInformation(id, course);

                if (updatedCourse == null)
                {
                    return NotFound();
                }

                return Ok(updatedCourse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }

        }

        // Tested
        // GET: api/Courses/5
        [Authorize(Roles= "Admin, Teacher, Student")]
        [HttpGet("{id}")]
        public async Task<ActionResult<DtoCourseResponse>> GetCourse(int id)
        {
            try
            {
                var course = await _course.GetCourseById(id);

                if (course == null)
                {
                    return NotFound();
                }

                return Ok(course);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }

        }
        
        // Tested
        // GET: api/Courses
        [Authorize(Roles = "Admin, Teacher, Student")]
        [HttpGet]
        public async Task<ActionResult<List<DtoCourseResponse>>> GetCourses()
        {
            try
            {
                var courses = await _course.GetAllCourses();
                return Ok(courses);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }

        }

        // Tested
        // GET: api/Courses/NotStarted
        [Authorize(Roles = "Student")]
        [HttpGet("NotStarted")]
        public async Task<ActionResult<List<DtoCourseResponse>>> GetCoursesNotStarted()
        {
            try
            {
                var courses = await _course.GetCoursesNotStarted(User);
                return Ok(courses);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }

        }

        // GET: api/Courses/Student/5/PreviousCourses
        [Authorize(Roles= "Student")]
        [HttpGet("Student/{id}/PreviousCourses")]
        public async Task<ActionResult<List<DtoPreviousCourseResponse>>> GetPreviousCoursesOfStudent()
        {
            try
            {
                var courses = await _course.GetPreviousCoursesOfStudent(User);
                return Ok(courses);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }

        }

        // GET: api/Courses/Student/5/AllCourses
        [Authorize(Roles = "Student")]
        [HttpGet("Student/{id}/AllCourses")]
        public async Task<ActionResult<List<DtoCourseResponse>>> GetCoursesOfStudent()
        {
            try
            {
                var courses = await _course.GetCoursesOfStudent(User);
                return Ok(courses);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }

        }

        // GET: api/Courses/Student/5/CurrentCourses
        [Authorize(Roles = "Student")]
        [HttpGet("Student/{id}/CurrentCourses")]
        public async Task<ActionResult<List<DtoCourseResponse>>> GetCurrentCoursesOfStudent()
        {
            try
            {
                var courses = await _course.GetCurrentCoursesOfStudent(User);
                return Ok(courses);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }

        }

        // GET: api/Courses/Teacher/5/AllCourses
        [Authorize(Roles = "Teacher")]
        [HttpGet("Teacher/{id}/AllCourses")]
        public async Task<ActionResult<List<DtoCourseResponse>>> GetCoursesOfTeacher()
        {
            try
            {
                var courses = await _course.GetCoursesOfTeacher(User);
                return Ok(courses);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Courses/Teacher/5/CurrentCourses
        [Authorize(Roles="Teacher")]
        [HttpGet("Teacher/{id}/CurrentCourses")]
        public async Task<ActionResult<List<DtoCourseResponse>>> GetCurrentCoursesOfTeacher()
        {
            try
            {
                var courses = await _course.GetCurrentCoursesOfTeacher(User);
                return Ok(courses);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/Courses/5/CalculateAverageGrade
        [Authorize(Roles = "Teacher")]
        [HttpPost("{id}/CalculateAverageGrade")]
        public async Task<ActionResult> CalculateAverageGrade(int id)
        {
            try
            {
                await _course.CalculateAverageGrade(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }
        private bool CourseExists(int id)
        {
            return _course.GetCourseById(id) != null;
        }

        // Tested
        //Delete: api/Courses/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _course.GetCourseById(id);
            if (course == null) return NotFound();
            try
            {
                await _course.DeleteCourse(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Teacher")]
        [HttpGet("{courseId}/grades")]
        public async Task<ActionResult<List<CourseGradesResponseDTO>>> GetStudentCourseGrades(int courseId)
        {
            try
            {
                var response= await _course.GetStudentCourseGrades(courseId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");

            }
        }

        [Authorize(Roles = "Student")]
        [HttpGet("{courseId}/grades/student")]
        public async Task<ActionResult<StudentGradesResponseDTO>> GetStudentGradesInCourse(int courseId)
        {
            try
            {
                var response = await _course.GetStudentGradesInCourse(courseId, User);
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

      
    }
}
