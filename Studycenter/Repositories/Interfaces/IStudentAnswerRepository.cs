using SCMS_back_end.Models;
using SCMS_back_end.Models.Dto;
using SCMS_back_end.Models.Dto.Quiz;
using SCMS_back_end.Models.Dto.Request;
using System.Security.Claims;

namespace SCMS_back_end.Repositories.Interfaces
{
    public interface IStudentAnswerRepository
    {
        Task<StudentAnswer> GetByIdAsync(int id);
        Task<IEnumerable<StudentAnswer>> GetByStudentIdAsync(int studentId);
        Task<IEnumerable<StudentAnswer>> GetByQuizIdAsync(int quizId);
        Task AddAsync(StudentAnswer studentAnswer);
        Task UpdateAsync(UpdateStudentAnswerRequestDto studentAnswer);
        Task DeleteAsync(int id);
        //Task<QuizResult> GetFinalScoreAsync(int studentId , int quizId ); // For Score
        Task<(int correctAnswersCount, int totalQuestionsCount, int quizMark)> CalculateScoreAsync(ClaimsPrincipal userPrincipal, int quizId);
        Task<SavedScoreDto> GetSavedScoreAsync(ClaimsPrincipal userPrincipal, int quizId);
        Task PostQuizResult(StudentQuizRequestDTO studentAnswers, ClaimsPrincipal userPrincipal);

        Task SaveAsync();
    }
}
