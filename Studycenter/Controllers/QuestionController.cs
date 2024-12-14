using SCMS_back_end.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SCMS_back_end.Repositories.Interfaces;
using SCMS_back_end.Models.Dto.Request;
using Microsoft.AspNetCore.Authorization;

namespace SCMS_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin, Teacher, Student")]

    public class QuestionController : ControllerBase
    {
        private readonly IQuestionRepository _questionService;

        public QuestionController(IQuestionRepository questionService)
        {
            _questionService = questionService;
        }

        [HttpGet("{quizId}")]
        public async Task<IActionResult> GetQuestionsByQuizId(int quizId)
        {
            var questions = await _questionService.GetQuestionsByQuizIdAsync(quizId);
            return Ok(questions);
        }

        [HttpGet("question/{questionId}")]
        public async Task<IActionResult> GetQuestionById(int questionId)
        {
            var question = await _questionService.GetQuestionByIdAsync(questionId);
            if (question == null)
                return NotFound();

            return Ok(question);
        }

        [HttpPost]
        public async Task<IActionResult> AddQuestion([FromBody] Question question)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _questionService.AddQuestionAsync(question);
            return CreatedAtAction(nameof(GetQuestionById), new { questionId = question.QuestionId }, question);
        }

        [HttpPut("{questionId}")]
        public async Task<IActionResult> UpdateQuestion(int questionId, [FromBody] QuestionUpdateDto questionDto)
        {
            try
            {
                // Call the service to update the question
                await _questionService.UpdateQuestionAsync(questionId, questionDto);

                // Return Ok (200) with a success message
                return Ok(new { message = "Question updated successfully" });
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Question with ID {questionId} not found.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
            }
        }





        [HttpDelete("{questionId}")]
        public async Task<IActionResult> DeleteQuestion(int questionId)
        {
            await _questionService.DeleteQuestionAsync(questionId);
            return NoContent();
        }
    }
}
