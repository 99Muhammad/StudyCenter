using SCMS_back_end.Models;

namespace SCMS_back_end.Repositories.Interfaces
{
    public interface IAnswerOptionRepository
    {
        Task<IEnumerable<AnswerOption>> GetAnswerOptionsByQuestionIdAsync(int questionId);
        Task<AnswerOption> GetAnswerOptionByIdAsync(int answerOptionId);
        Task AddAnswerOptionAsync(AnswerOption answerOption);
        Task UpdateAnswerOptionAsync(AnswerOption answerOption);
        Task DeleteAnswerOptionAsync(int answerOptionId);
    }
}
