using SCMS_back_end.Models;
using SCMS_back_end.Models.Dto.Quiz;
using SCMS_back_end.Models.Dto.Request;
using System.Security.Claims;
using SCMS_back_end.Models.Dto.Response;

namespace SCMS_back_end.Repositories.Interfaces
{
    public interface IQuizRepository
    {
        Task<IEnumerable<Quiz>> GetAllQuizzesAsync();
        Task<Quiz> GetQuizByIdAsync(int quizId);
        Task AddQuizAsync(Quiz quiz);
        Task UpdateQuizAsync(int quizId, QuizUpdateDto quizDto);
        Task DeleteQuizAsync(int quizId);
        Task<IEnumerable<StudentQuizResponseDTO>> GetCourseQuizzesByStudent(int courseId, ClaimsPrincipal userPrincipal);
        Task<IEnumerable<DtoQuizSubmissions>> GetQuizSubmissionAsync(int quizId);
    }
}
