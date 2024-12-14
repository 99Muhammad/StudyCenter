using SCMS_back_end.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SCMS_back_end.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using SCMS_back_end.Data;
using SCMS_back_end.Models.Dto.Request;
using SCMS_back_end.Models.Dto.Response;
using SCMS_back_end.Services;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace SCMS_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin, Teacher, Student")]
    public class QuizController : ControllerBase
    {
        private readonly IQuizRepository _quizService;

        public QuizController(IQuizRepository quizService)
        {
            _quizService = quizService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllQuizzes()
        {
            var quizzes = await _quizService.GetAllQuizzesAsync();
            return Ok(quizzes);
        }

        [HttpGet("{quizId}")]
        public async Task<IActionResult> GetQuizById(int quizId)
        {

            try
            {
                var quiz = await _quizService.GetQuizByIdAsync(quizId);
                return Ok(quiz);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(); // Quiz not found
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddQuiz([FromBody] Quiz quiz)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _quizService.AddQuizAsync(quiz);
            return CreatedAtAction(nameof(GetQuizById), new { quizId = quiz.QuizId }, quiz);
        }

        [HttpPut("{quizId}")]
        public async Task<IActionResult> UpdateQuiz(int quizId, [FromBody] QuizUpdateDto quizDto)
        {
            try
            {
                // Update the quiz
                await _quizService.UpdateQuizAsync(quizId, quizDto);
                return NoContent(); // Indicates successful update
            }
            catch (KeyNotFoundException)
            {
                return NotFound(); // Quiz not found
            }
            catch (Exception ex)
            {
                // Log the exception (if logging is set up)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpDelete("{quizId}")]
        public async Task<IActionResult> DeleteQuiz(int quizId)
        {
            try
            {
                await _quizService.DeleteQuizAsync(quizId);
                // Return a JSON response with a success message
                return new JsonResult(new { message = "Deleted Successfully" })
                {
                    StatusCode = 200 // OK
                };
            }
            catch (KeyNotFoundException)
            {
                return NotFound(); // Quiz not found
            }
            catch (Exception ex)
            {
                // Log the exception (if logging is set up)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("quizSubmission/{quizId}")]
        public async Task<IActionResult> GetQuizSubmission(int quizId)
        {
            var quizSubmissions = await _quizService.GetQuizSubmissionAsync(quizId);
            return Ok(quizSubmissions);
        }
        [HttpGet("course/{courseId}")]
        public async Task<IActionResult> GetCourseQuizzesByStudent(int courseId)
        {
            try
            {
                var quizzes = await _quizService.GetCourseQuizzesByStudent(courseId, User);
                return Ok(quizzes);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
