
using SCMS_back_end.Models;
using Microsoft.EntityFrameworkCore;
using SCMS_back_end.Data;
using SCMS_back_end.Repositories.Interfaces;
using SCMS_back_end.Models.Dto.Request;

namespace SCMS_back_end.Services
{
    public class QuestionService : IQuestionRepository
    {
        private readonly StudyCenterDbContext _context;

        public QuestionService(StudyCenterDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Question>> GetQuestionsByQuizIdAsync(int quizId)
        {
            return await _context.Questions
                                 .Where(q => q.QuizId == quizId)
                                 .Include(q => q.AnswerOptions)
                                 .ToListAsync();
        }

        public async Task<Question> GetQuestionByIdAsync(int questionId)
        {
            return await _context.Questions
                                 .Include(q => q.AnswerOptions)
                                 .FirstOrDefaultAsync(q => q.QuestionId == questionId);
        }

        public async Task AddQuestionAsync(Question question)
        {
            _context.Questions.Add(question);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateQuestionAsync(int questionId, QuestionUpdateDto questionDto)
        {
            var existingQuestion = await _context.Questions.FindAsync(questionId);
            if (existingQuestion == null)
            {
                throw new Exception($"Question with ID {questionId} not found.");
            }

            // Map the properties from the DTO to the entity
            existingQuestion.Text = questionDto.Text;
            existingQuestion.QuizId = questionDto.QuizId;

            _context.Questions.Update(existingQuestion);
            await _context.SaveChangesAsync();
        }




        public async Task DeleteQuestionAsync(int questionId)
        {
            var question = await GetQuestionByIdAsync(questionId);
            if (question != null)
            {
                _context.Questions.Remove(question);
                await _context.SaveChangesAsync();
            }
        }
    }
}
