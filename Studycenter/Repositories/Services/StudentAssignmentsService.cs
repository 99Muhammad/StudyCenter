using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SCMS_back_end.Data;
using SCMS_back_end.Models;
using SCMS_back_end.Models.Dto.Request;
using SCMS_back_end.Models.Dto.Request.Assignment;
using SCMS_back_end.Models.Dto.Response;
using SCMS_back_end.Repositories.Interfaces;
using SCMS_back_end.Repositories.Services;
namespace SCMS_back_end.Services
{
    public class StudentAssignmentsService : IStudentAssignments
    {
        private readonly StudyCenterDbContext _context;

        public StudentAssignmentsService(StudyCenterDbContext context)
        {
            _context = context;
        }

        // Student Submission Method
        public async Task<StudentAssignment> AddStudentAssignmentSubmissionAsync(StudentAssignmentSubmissionDtoRequest dto, ClaimsPrincipal userPrincipal)
        {
            var userIdClaim = userPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userIdClaim);
            if (student == null)
            {
                throw new InvalidOperationException("Student not found.");
            }

            var assignmentExists = await _context.Assignments
                .AnyAsync(a => a.AssignmentId == dto.AssignmentId);

            if (!assignmentExists)
            {
                throw new Exception("Assignment not found. Please provide a valid AssignmentId.");
            }

            var existingRecord = await _context.StudentAssignments
                .FirstOrDefaultAsync(sa => sa.StudentId == student.StudentId && sa.AssignmentId == dto.AssignmentId);

            if (existingRecord != null && !string.IsNullOrEmpty(existingRecord.Submission))
            {
                throw new Exception("Submission already exists. You cannot submit more than once.");
            }

            // Handle file upload
            string filePath = null;
            if (dto.File != null)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var fileExtension = Path.GetExtension(dto.File.FileName);
                var newFileName = $"{Path.GetFileNameWithoutExtension(dto.File.FileName)}_{Guid.NewGuid()}{fileExtension}";
                filePath = Path.Combine(uploadsFolder, newFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.File.CopyToAsync(fileStream);
                }
            }

            if (existingRecord != null)
            {
                existingRecord.Submission = dto.Submission;
                existingRecord.SubmissionDate = DateTime.Now;
                if (!string.IsNullOrEmpty(filePath))
                {
                    existingRecord.FilePath = filePath; // Store file path
                }
                _context.StudentAssignments.Update(existingRecord);
            }
            else
            {
                var newAssignment = new StudentAssignment
                {
                    AssignmentId = dto.AssignmentId,
                    StudentId = student.StudentId,
                    SubmissionDate = DateTime.Now,
                    Submission = dto.Submission,
                    FilePath = filePath // Store file path
                };

                await _context.StudentAssignments.AddAsync(newAssignment);
                existingRecord = newAssignment;
            }

            await _context.SaveChangesAsync();
            return existingRecord;
        }
        // Teacher Feedback Method
        public async Task<StudentAssignment> AddStudentAssignmentFeedbackAsync(TeacherAssignmentFeedbackDtoRequest dto)
        {
            var student = await _context.Students.FindAsync(dto.studentId);
            if (student == null)
            {
                throw new InvalidOperationException("Student not found.");
            }
            var assignment = await _context.Assignments.FindAsync(dto.assignmentId);
            if (assignment == null)
            {
                throw new InvalidOperationException("Assignment not found.");
            }
            var studentAssignment= new StudentAssignment
            {
                AssignmentId = dto.assignmentId,
                StudentId = dto.studentId,
                Grade = dto.Grade.Value,
                Feedback = dto.Feedback
            };

            await _context.StudentAssignments.AddAsync(studentAssignment);
            await _context.SaveChangesAsync();


            // calculate grades for the course students
            var courseId = assignment.CourseId;
            var courseService = new CourseService(_context);
            await courseService.CalculateCourseScores(courseId);

            return studentAssignment;
        }

        public async Task<StudentAssignment> AddGradeForNonSubmittedAssignmentAsync(DtoGradeNotSubmitted notSubmitted)
        {
            var assignmentExists = await _context.Assignments
                .AnyAsync(a => a.AssignmentId == notSubmitted.assignmentId);

            if (!assignmentExists)
            {
                throw new Exception("Assignment not found. Please provide a valid AssignmentId.");
            }

            var studentExists = await _context.Students
                .AnyAsync(s => s.StudentId == notSubmitted.studentId);

            if (!studentExists)
            {
                throw new Exception("Student not found. Please provide a valid StudentId.");
            }

            var existingRecord = await _context.StudentAssignments
                .FirstOrDefaultAsync(sa => sa.StudentId == notSubmitted.studentId && sa.AssignmentId == notSubmitted.assignmentId);

            if (existingRecord != null && !string.IsNullOrEmpty(existingRecord.Submission))
            {
                throw new Exception("Submission already exists. You cannot grade a submitted assignment using this method.");
            }

            if (existingRecord == null)
            {
                existingRecord = new StudentAssignment
                {
                    AssignmentId = notSubmitted.assignmentId,
                    StudentId = notSubmitted.studentId,
                    Grade = notSubmitted.grade,
                    Feedback = notSubmitted.feedback,
                    SubmissionDate = DateTime.Now
                };

                await _context.StudentAssignments.AddAsync(existingRecord);
            }
            else
            {
                existingRecord.Grade = notSubmitted.grade;
                existingRecord.Feedback = notSubmitted.feedback;
                _context.StudentAssignments.Update(existingRecord);
            }

            await _context.SaveChangesAsync();
            return existingRecord;
        }


        public async Task<StudentAssignment> UpdateStudentAssignmentAsync(int studentAssignmentId, int? grade, string feedback)
        {
            //var studentAssignment = await _context.StudentAssignments.FindAsync(studentAssignmentId);
            var studentAssignment = await _context.StudentAssignments.Include(sa => sa.Assignment) 
           .FirstOrDefaultAsync(sa => sa.StudentAssignmentId == studentAssignmentId);

            if (studentAssignment != null)
            {
                if (grade.HasValue)
                    studentAssignment.Grade = grade.Value;

                if (!string.IsNullOrEmpty(feedback))
                    studentAssignment.Feedback = feedback;

                _context.StudentAssignments.Update(studentAssignment);
                await _context.SaveChangesAsync();

                // calculate grades for the course students
                var courseId = studentAssignment.Assignment.CourseId;
                var courseService = new CourseService(_context);
                await courseService.CalculateCourseScores(courseId);
            }
            return studentAssignment;
        }

        public async Task<StudentAssignmentDtoResponse> GetStudentAssignmentByIdAsync(int studentAssignmentId)
        {
            var studentAssignment = await _context.StudentAssignments
                .Include(sa => sa.Assignment)
                .Include(sa => sa.Student)
                .FirstOrDefaultAsync(sa => sa.StudentAssignmentId == studentAssignmentId);

            if (studentAssignment == null)
            {
                return null;
            }

            var responseDto = new StudentAssignmentDtoResponse
            {
                StudentAssignmentId = studentAssignment.StudentAssignmentId,
                AssignmentId = studentAssignment.AssignmentId,
                StudentId = studentAssignment.StudentId,
                SubmissionDate = studentAssignment.SubmissionDate, // No error now
                Submission = studentAssignment.Submission,
                Grade = studentAssignment.Grade,
                Feedback = studentAssignment.Feedback,
                FilePath = studentAssignment.FilePath
            };

            return responseDto;
        }

        public async Task<StudentAssignmentDtoResponse> GetStudentAssignmentByAssignmentAndStudentAsync(int assignmentId, int studentId)
        {
            var studentAssignment = await _context.StudentAssignments
                .Include(sa => sa.Assignment)
                .Include(sa => sa.Student)
                .FirstOrDefaultAsync(sa => sa.AssignmentId == assignmentId && sa.StudentId == studentId);

            if (studentAssignment == null)
            {
                return null;
            }

            var responseDto = new StudentAssignmentDtoResponse
            {
                StudentAssignmentId = studentAssignment.StudentAssignmentId,
                AssignmentId = studentAssignment.AssignmentId,
                StudentId = studentAssignment.StudentId,
                SubmissionDate = studentAssignment.SubmissionDate, // No error now
                Submission = studentAssignment.Submission,
                Grade = studentAssignment.Grade,
                Feedback = studentAssignment.Feedback,
                FilePath = studentAssignment.FilePath
            };

            return responseDto;
        }
    }
}
    