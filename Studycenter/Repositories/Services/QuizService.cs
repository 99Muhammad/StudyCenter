
using SCMS_back_end.Models;
using Microsoft.EntityFrameworkCore;
using SCMS_back_end.Data;
using SCMS_back_end.Repositories.Interfaces;
using SCMS_back_end.Models.Dto.Request;
using System.Security.Claims;
using SCMS_back_end.Models.Dto.Quiz;
using SCMS_back_end.Repositories.Services;
using SCMS_back_end.Models.Dto.Response;

namespace SCMS_back_end.Services
{
    public class QuizService : IQuizRepository
    {
        private readonly StudyCenterDbContext _context;

        public QuizService(StudyCenterDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Quiz>> GetAllQuizzesAsync()
        {
            return await _context.Quizzes.Include(q => q.Questions)
                .ThenInclude(q => q.AnswerOptions)
                .Include(q=>q.QuizResult)
                .ToListAsync();
        }

        public async Task<Quiz> GetQuizByIdAsync(int quizId)
        {
            var quiz = await _context.Quizzes
                                     .Include(q => q.Questions)
                                     .ThenInclude(q => q.AnswerOptions)
                                     .FirstOrDefaultAsync(q => q.QuizId == quizId);

            if (quiz == null)
            {
                throw new KeyNotFoundException("Quiz not found");
            }

            return quiz;
        }

        public async Task AddQuizAsync(Quiz quiz)
        {
            _context.Quizzes.Add(quiz);
            await _context.SaveChangesAsync();

            // calculate grades for the course students
            var courseId= quiz.CourseId ?? 1;
            var courseService= new CourseService(_context);
            await courseService.CalculateCourseMark(courseId);
            await courseService.CalculateCourseScores(courseId);
        }

        public async Task UpdateQuizAsync(int quizId, QuizUpdateDto quizDto)
        {
            var existingQuiz = await _context.Quizzes.FindAsync(quizId);
            if (existingQuiz == null)
            {
                throw new KeyNotFoundException("Quiz not found");
            }

            // Update the properties
            existingQuiz.Title = quizDto.Title;
            existingQuiz.Duration = quizDto.Duration;
            existingQuiz.IsVisible = quizDto.IsVisible;
            existingQuiz.CourseId = quizDto.CourseId;

            await _context.SaveChangesAsync();

            // calculate grades for the course students
            var courseId = existingQuiz.CourseId ?? 1;
            var courseService = new CourseService(_context);
            await courseService.CalculateCourseMark(courseId);
            await courseService.CalculateCourseScores(courseId);
        }
        public async Task DeleteQuizAsync(int quizId)
        {
            var quiz = await _context.Quizzes
                .Include(q=>q.QuizResult)
                .Include(q=>q.StudentQuizzes)
                .Include(q=>q.Questions)
                .ThenInclude(q=>q.AnswerOptions)
                .ThenInclude(a=>a.StudentAnswers)
                .ThenInclude(sa=>sa.StudentAnswerResult)
                .FirstOrDefaultAsync(q => q.QuizId == quizId);


            if (quiz != null)
            { 
                // Delete StudentAnswers
                foreach (var question in quiz.Questions)
                {
                    foreach (var answerOption in question.AnswerOptions)
                    {
                        _context.StudentAnswers.RemoveRange(answerOption.StudentAnswers);
                    }
                }

                // Delete AnswerOptions
                foreach (var question in quiz.Questions)
                {
                    _context.AnswerOptions.RemoveRange(question.AnswerOptions);
                }

                // Delete Questions
                _context.Questions.RemoveRange(quiz.Questions);

                // Delete QuizResults
                _context.QuizResults.RemoveRange(quiz.QuizResult);

                // Delete the Quiz
                _context.Quizzes.Remove(quiz);

                await _context.SaveChangesAsync();

                // calculate grades for the course students
                var courseId = quiz.CourseId ?? 1;
                var courseService = new CourseService(_context);
                await courseService.CalculateCourseMark(courseId);
                await courseService.CalculateCourseScores(courseId);
            }
        }
        public async Task<IEnumerable<StudentQuizResponseDTO>> GetCourseQuizzesByStudent(int courseId, ClaimsPrincipal userPrincipal)
        {
            var userIdClaim = userPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userIdClaim);
            if (student == null)
            {
                throw new InvalidOperationException("Student not found.");
            }
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.CourseId == courseId);
            if (course == null)
            {
                throw new InvalidOperationException("Course not found.");
            }
            var quizzes= await _context.Quizzes.Include(q=>q.QuizResult).Where(q => q.CourseId == courseId && q.IsVisible).
                Select(q => new StudentQuizResponseDTO
                {
                    QuizId = q.QuizId,
                    Title = q.Title,
                    Duration = q.Duration,
                    IsVisible = q.IsVisible,
                    CourseId = q.CourseId,
                    Mark = q.Mark,
                    StartTime = q.StartTime,
                    EndTime = q.EndTime,
                    QuizResult = q.QuizResult.FirstOrDefault(qr => qr.StudentId == student.StudentId)
                }).ToListAsync();
            return quizzes;
        }

        public async Task<IEnumerable<DtoQuizSubmissions>> GetQuizSubmissionAsync(int quizId)
        {
            var quizResults = await _context.QuizResults.Include(qr => qr.Student)
            .Where(qr => qr.QuizId == quizId)
            .Select(qr => new DtoQuizSubmissions
            {
                Id = qr.Id,
                StudentName = qr.Student.FullName,
                SubmissionDate = qr.SubmittedAt,
                Grade = qr.Score
            })
            .ToListAsync();

            return quizResults;

        }
    }
}
