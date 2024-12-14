using SCMS_back_end.Models;
using SCMS_back_end.Models.Dto.Request;

namespace SCMS_back_end.Repositories.Interfaces
{
    public interface IQuestionRepository
    {
        Task<IEnumerable<Question>> GetQuestionsByQuizIdAsync(int quizId);
        Task<Question> GetQuestionByIdAsync(int questionId);
        Task AddQuestionAsync(Question question);
        Task UpdateQuestionAsync(int questionId, QuestionUpdateDto questionDto);
        Task DeleteQuestionAsync(int questionId);
    }
}
