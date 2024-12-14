using SCMS_back_end.Data;
using SCMS_back_end.Models;
using SCMS_back_end.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SCMS_back_end.Models.Dto.Request;
using SCMS_back_end.Models.Dto;
using System.Security.Claims;
using SCMS_back_end.Models.Dto.Quiz;
using SendGrid.Helpers.Errors.Model;

namespace SCMS_back_end.Repositories.Services
{
    public class StudentAnswerRepository : IStudentAnswerRepository
    {
        private readonly StudyCenterDbContext _context;

        public StudentAnswerRepository(StudyCenterDbContext context)
        {
            _context = context;
        }

        public async Task<StudentAnswer> GetByIdAsync(int id)
        {
            return await _context.StudentAnswers.FindAsync(id);
        }

        public async Task<IEnumerable<StudentAnswer>> GetByStudentIdAsync(int studentId)
        {
            return await _context.StudentAnswers
                                 .Where(sa => sa.StudentId == studentId)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<StudentAnswer>> GetByQuizIdAsync(int quizId)
        {
            return await _context.StudentAnswers
                                 .Where(sa => sa.QuizId == quizId)
                                 .ToListAsync();
        }

       
        
        public async Task AddAsync(StudentAnswer studentAnswer)
        {

            // Ensure related entities are loaded
            await _context.StudentAnswers.AddAsync(studentAnswer);
            await _context.SaveChangesAsync(); // Save the new answer first

        }


        public async Task UpdateAsync(UpdateStudentAnswerRequestDto studentAnswer)
        {
            var existingStudentAnswer = await _context.StudentAnswers.FindAsync(studentAnswer.Id);
            if (existingStudentAnswer != null)
            {
                // Update properties
                existingStudentAnswer.QuizId = studentAnswer.QuizId;
                existingStudentAnswer.QuestionId = studentAnswer.QuestionId;
                existingStudentAnswer.SelectedAnswerOptionId = studentAnswer.SelectedAnswerOptionId;
                existingStudentAnswer.StudentId = studentAnswer.StudentId;
                existingStudentAnswer.SubmittedAt = studentAnswer.SubmittedAt;

                _context.StudentAnswers.Update(existingStudentAnswer);
            }
            else
            {
                throw new KeyNotFoundException("StudentAnswer not found");
            }
        }


        public async Task DeleteAsync(int id)
        {
            var studentAnswer = await _context.StudentAnswers.FindAsync(id);
            if (studentAnswer != null)
            {
                _context.StudentAnswers.Remove(studentAnswer);
            }
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<(int correctAnswersCount, int totalQuestionsCount, int quizMark)> CalculateScoreAsync(ClaimsPrincipal userPrincipal, int quizId)
        {
            var userIdClaim = userPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userIdClaim);
            if (student == null)
            {
                throw new InvalidOperationException("Student not found.");
            }

            // Count the number of correct answers submitted by the student for the quiz
            var correctAnswersCount = await _context.StudentAnswers
                .Where(sa => sa.StudentId == student.StudentId && sa.QuizId == quizId && sa.SelectedAnswerOption.IsCorrect)
                .CountAsync();

            // Count the total number of questions for the quiz
            var totalQuestionsCount = await _context.Questions
                .Where(q => q.QuizId == quizId)
                .CountAsync();

            // Retrieve the Quiz
            var quiz = await _context.Quizzes
                .Where(q => q.QuizId == quizId)
                .FirstOrDefaultAsync();

            if (quiz == null)
            {
                throw new KeyNotFoundException("Quiz not found");
            }

            var quizMark = quiz.Mark;

            // Save the result in the QuizResult table
            var quizResult = await _context.QuizResults
                .FirstOrDefaultAsync(qr => qr.StudentId == student.StudentId && qr.QuizId == quizId);

            if (quizResult == null)
            {
                // Create a new QuizResult if it doesn't exist
                quizResult = new QuizResult
                {
                    StudentId = student.StudentId,
                    QuizId = quizId,
                    NumbersOfCorrectAnswers = correctAnswersCount,
                    Score = (int)Math.Round((decimal)correctAnswersCount / totalQuestionsCount * quizMark, 2),
                    SubmittedAt = DateTime.Now
                };

                await _context.QuizResults.AddAsync(quizResult);
            }
            else
            {
                // Update the existing QuizResult
                quizResult.NumbersOfCorrectAnswers = correctAnswersCount;
                quizResult.Score = (int)Math.Round((decimal)correctAnswersCount / totalQuestionsCount * quizMark, 2);

                _context.QuizResults.Update(quizResult);
            }

            await _context.SaveChangesAsync();

            // calculate grades for the course students
            var courseId = quiz.CourseId ?? 1;
            var courseService = new CourseService(_context);
            await courseService.CalculateCourseScores(courseId);

            return (correctAnswersCount, totalQuestionsCount, quizResult.Score);
        }

        private async Task<(int correctAnswersCount, int totalQuestionsCount, int quizMark)> CalculateScoreAsyncInProgram(int studentId, int quizId)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == studentId);
            if (student == null)
            {
                throw new InvalidOperationException("Student not found.");
            }

            // Count the number of correct answers submitted by the student for the quiz
            var correctAnswersCount = await _context.StudentAnswers
                .Where(sa => sa.StudentId == student.StudentId && sa.QuizId == quizId && sa.SelectedAnswerOption.IsCorrect)
                .CountAsync();

            // Count the total number of questions for the quiz
            var totalQuestionsCount = await _context.Questions
                .Where(q => q.QuizId == quizId)
                .CountAsync();

            // Retrieve the Quiz
            var quiz = await _context.Quizzes
                .Where(q => q.QuizId == quizId)
                .FirstOrDefaultAsync();

            if (quiz == null)
            {
                throw new KeyNotFoundException("Quiz not found");
            }

            var quizMark = quiz.Mark;

            // Save the result in the QuizResult table
            var quizResult = await _context.QuizResults
                .FirstOrDefaultAsync(qr => qr.StudentId == student.StudentId && qr.QuizId == quizId);

            if (quizResult == null)
            {
                // Create a new QuizResult if it doesn't exist
                quizResult = new QuizResult
                {
                    StudentId = student.StudentId,
                    QuizId = quizId,
                    NumbersOfCorrectAnswers = correctAnswersCount,
                    Score = (int)Math.Round((decimal)correctAnswersCount / totalQuestionsCount * quizMark, 2)
                };

                await _context.QuizResults.AddAsync(quizResult);
            }
            else
            {
                // Update the existing QuizResult
                quizResult.NumbersOfCorrectAnswers = correctAnswersCount;
                quizResult.Score = (int)Math.Round((decimal)correctAnswersCount / totalQuestionsCount * quizMark, 2);

                _context.QuizResults.Update(quizResult);
            }

            await _context.SaveChangesAsync();

            // calculate grades for the course students
            var courseId = quiz.CourseId ?? 1;
            var courseService = new CourseService(_context);
            await courseService.CalculateCourseScores(courseId);

            return (correctAnswersCount, totalQuestionsCount, quizResult.Score);
        }


        // 2. Retrieve the saved score from the QuizResult
        public async Task<SavedScoreDto> GetSavedScoreAsync(ClaimsPrincipal userPrincipal, int quizId)
        {
            var userIdClaim = userPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userIdClaim);
            if (student == null)
            {
                throw new InvalidOperationException("Student not found.");
            }

            var quizResult = await _context.QuizResults
                .FirstOrDefaultAsync(qr => qr.StudentId == student.StudentId && qr.QuizId == quizId);

            if (quizResult == null)
            {
                throw new KeyNotFoundException("Quiz result not found");
            }

            var studentAnswers = await _context.StudentAnswers
                .Include(sa=>sa.SelectedAnswerOption)
                .Where(sa => sa.StudentId == student.StudentId && sa.QuizId == quizId)
                .ToListAsync();

            // Log the number of student answers found
            if (studentAnswers.Count == 0)
            {
                // Return a message indicating no answers were found
                return new SavedScoreDto
                {
                    CorrectAnswersCount = 0,
                    TotalQuestionsCount = 0,
                    QuizMark = 0,
                    Score = quizResult.Score,
                    StudentAnswers = Enumerable.Empty<StudentAnswer>()
                };
            }

            var correctAnswersCount = studentAnswers.Count(sa => sa.SelectedAnswerOption != null && sa.SelectedAnswerOption.IsCorrect);
            var totalQuestionsCount = await _context.Questions.CountAsync(q => q.QuizId == quizId);
            var quiz = await _context.Quizzes.FindAsync(quizId);

            if (quiz == null)
            {
                throw new KeyNotFoundException("Quiz not found");
            }

            var quizMark = quiz.Mark;

            //return (correctAnswersCount, totalQuestionsCount, quizMark, quizResult.Score, studentAnswers);
            return new SavedScoreDto
            {
                CorrectAnswersCount = correctAnswersCount,
                TotalQuestionsCount = totalQuestionsCount,
                QuizMark = quizMark,
                Score = quizResult.Score,
                StudentAnswers = studentAnswers
            };
        }

        public async Task PostQuizResult(StudentQuizRequestDTO studentAnswers, ClaimsPrincipal userPrincipal)
        {
            var userIdClaim = userPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userIdClaim);
            if (student == null)
            {
                throw new KeyNotFoundException("Student not found.");
            }
            var quiz= await _context.Quizzes.FindAsync(studentAnswers.QuizId);
            if (quiz == null)
            {
                throw new KeyNotFoundException("Quiz not found.");
            }
            foreach (var studentAnswer in studentAnswers.studentAnswers)
            {
                await postStudentAnswer(new StudentAnswer
                {
                    StudentId = student.StudentId,
                    QuizId = studentAnswers.QuizId,
                    QuestionId = studentAnswer.questionId,
                    SelectedAnswerOptionId = studentAnswer.selectedAnswerOptionId,
                    SubmittedAt= DateTime.Now
                });
            }
           await CalculateScoreAsyncInProgram(student.StudentId, quiz.QuizId);
        }

        private async Task postStudentAnswer(StudentAnswer studentAnswer)
        {
            var question = await _context.Questions.FindAsync(studentAnswer.QuestionId);
            if (question == null)
            {
                throw new KeyNotFoundException("Question not found.");
            }
            var option = await _context.AnswerOptions.FindAsync(studentAnswer.SelectedAnswerOptionId);
            if (option == null)
            {
                throw new KeyNotFoundException("Answer option not found.");
            }
            await _context.StudentAnswers.AddAsync(studentAnswer);
            await _context.SaveChangesAsync(); 

        }
    }
}