using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using SCMS_back_end.Data;
using SCMS_back_end.Models;
using SCMS_back_end.Models.Dto.Request.Assignment;
using SCMS_back_end.Models.Dto.Request.Teacher;
using SCMS_back_end.Models.Dto.Response.Assignment;
using SCMS_back_end.Repositories.Interfaces;
using System.Security.Claims;

namespace SCMS_back_end.Repositories.Services
{
    public class AssignmentService : IAssignment
    {
        private readonly StudyCenterDbContext _context;
        private UserManager<User> _userManager;

        public AssignmentService(StudyCenterDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        public async Task<DtoAddAssignmentResponse> AddAssignment(DtoAddAssignmentRequest AssignmentDto)
        {
            var courseExists = await _context.Courses.AnyAsync(c => c.CourseId == AssignmentDto.CourseId);
            if (!courseExists)
            {
                throw new Exception("Course does not exist.");
            }
            var NewAssignment = new Assignment()
            {
               // assignmentId = AssignmentDto.assignmentId,
                CourseId = AssignmentDto.CourseId,
                AssignmentName = AssignmentDto.AssignmentName,
                DueDate = AssignmentDto.DueDate,
                Description = AssignmentDto.Description,
                Visible = AssignmentDto.Visible,
                FullMark = AssignmentDto.FullMark
            };

            _context.Assignments.Add(NewAssignment);
            await _context.SaveChangesAsync();

            var Response = new DtoAddAssignmentResponse()
            {
                AssignmentName = AssignmentDto.AssignmentName,
                DueDate = AssignmentDto.DueDate,
                Description = AssignmentDto.Description,
                Visible = AssignmentDto.Visible,
                FullMark = AssignmentDto.FullMark
            };

            // calculate grades for the course students
            var courseId = AssignmentDto.CourseId;
            var courseService = new CourseService(_context);
            await courseService.CalculateCourseMark(courseId);
            await courseService.CalculateCourseScores(courseId);

            return Response;
        }

        public async Task<List<DtoAddAssignmentResponse>> GetAllAssignmentsByCourseID(int CourseID)
        {
            var allAssignments = await _context.Courses
             .Where(x => x.CourseId == CourseID)
             .SelectMany(x => x.Assignments)
             .Include(a=>a.StudentAssignments)
             .ToListAsync();


            //if(allAssignments.Count<=0)
            //{
            //   throw new ArgumentException("Invalid Course ID", nameof(CourseID));
            //}

            // Map assignmentsWithStudentAssignment to DTOs
            var assignmentDtos = allAssignments.Select(a => new DtoAddAssignmentResponse
            {
                AssignmentId = a.AssignmentId,
                AssignmentName = a.AssignmentName,
                DueDate = a.DueDate,
                Description = a.Description,
                Visible = a.Visible,
                FullMark = a.FullMark,
                submissions=a.StudentAssignments.Count(),
            }).ToList();

            return assignmentDtos;

        }

        public async Task<DtoUpdateAssignmentResponse> UpdateAssignmentByID(int AssignmentID, DtoUpdateAssignmentRequest AssignmentDto)
        {
            var Assignment = await _context.Assignments.FirstOrDefaultAsync(x => x.AssignmentId == AssignmentID);

            if (Assignment == null)
            {
                throw new ArgumentException("Invalid Assignment ID", nameof(AssignmentID));
            }

            if (!string.IsNullOrEmpty(AssignmentDto.AssignmentName))
                Assignment.AssignmentName = AssignmentDto.AssignmentName;

            if (AssignmentDto.DueDate != Convert.ToDateTime("01/01/0001 00:00:00"))
                Assignment.DueDate = AssignmentDto.DueDate;

            //if (string.IsNullOrEmpty(AssignmentDto.Description))
                Assignment.Description = AssignmentDto.Description;

            if (AssignmentDto.FullMark != 0)
                Assignment.FullMark = AssignmentDto.FullMark;

            Assignment.Visible = AssignmentDto.Visible;

            await _context.SaveChangesAsync();

            var Response = new DtoUpdateAssignmentResponse()
            {
                AssignmentName = Assignment.AssignmentName,
                DueDate = AssignmentDto.DueDate,
                Description = AssignmentDto.Description,
                Visible = Assignment.Visible,
                FullMark = AssignmentDto.FullMark
            };

            // calculate grades for the course students
            var courseId = Assignment.CourseId;
            var courseService = new CourseService(_context);
            await courseService.CalculateCourseMark(courseId);
            await courseService.CalculateCourseScores(courseId);

            return Response;
        }

        public async Task<DtoAddAssignmentResponse> GetAllAssignmentInfoByAssignmentID(int AssignmentID)
        {
            var Assignment = await _context.Assignments.FirstOrDefaultAsync(x => x.AssignmentId == AssignmentID);

            if (Assignment == null)
            {
                throw new KeyNotFoundException($"Assignment with ID {AssignmentID} not found.");
            }
            var AssignmentDto = new DtoAddAssignmentResponse()
            {
                AssignmentId= Assignment.AssignmentId,
                AssignmentName = Assignment.AssignmentName,
                DueDate = Assignment.DueDate,
                Description = Assignment.Description,
                Visible = Assignment.Visible,
                FullMark = Assignment.FullMark
            };
            return AssignmentDto;

        }
        public async Task DeleteAssignment(int AssignmentID)
        {
            var AssignmentToDelete =await _context.Assignments
                .FirstOrDefaultAsync(x => x.AssignmentId == AssignmentID);

            if(AssignmentToDelete==null)
            {
                throw new Exception("Assignment not found.");
            }

            _context.Assignments.Remove(AssignmentToDelete);
            await _context.SaveChangesAsync();

            var Response = new DtoDeleteAssignmentResponse()
            {
                AssignmentName = AssignmentToDelete.AssignmentName,
                DueDate = AssignmentToDelete.DueDate,
                Description = AssignmentToDelete.Description,

            };

            // calculate grades for the course students
            var courseId = AssignmentToDelete.CourseId;
            var courseService = new CourseService(_context);
            await courseService.CalculateCourseMark(courseId);
            await courseService.CalculateCourseScores(courseId);

        }

        public async Task<List<DtoStudentAssignmentResponse>> GetStudentAssignmentsByCourseId(int courseId, ClaimsPrincipal userPrincipal)
        {
            // get all assignmentsWithStudentAssignment in s course 
            // get the student assignment record for each assignment 
           // var user = await _userManager.GetUserAsync(userPrincipal);

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

            var assignmentsWithStudentAssignment = await _context.Assignments
                .Include(a => a.StudentAssignments)
                .Where(a => a.CourseId == courseId && a.Visible == true)
                .Select(a => new DtoStudentAssignmentResponse
                {
                    AssignmentId = a.AssignmentId,
                    AssignmentName = a.AssignmentName,
                    DueDate = a.DueDate,
                    Mark=a.FullMark,
                    StudentAssignment = a.StudentAssignments
                    
                .Where(sa => sa.AssignmentId == a.AssignmentId && sa.StudentId == student.StudentId)
                .Select(sa => new DtoStudentAssignmentDetails
                {
                    StudentAssignmentId = sa.StudentAssignmentId,
                    Grade = sa.Grade,
                    Feedback = sa.Feedback,
                    SubmissionDate = sa.SubmissionDate
                }).FirstOrDefault()
                })
                .ToListAsync();
            
            return assignmentsWithStudentAssignment;
        }
        
        public async Task<List<DtoStudentSubmissionResponse>> GetStudentsSubmissionByAssignmentId(int assignmentId)
        {
            //get all studentsWithStudentAssignment with the student assignment record for each student 

            //get studentsWithStudentAssignment in s course 
            //get student assignment reocrd for each student by student id and assignment id 
             var assignment= _context.Assignments.FirstOrDefault(a => a.AssignmentId == assignmentId);
             if(assignment == null)
             {
                 throw new InvalidOperationException("Assignment not found.");
             }

            //var students = await _context.Students
            //    //.Include(s => s.StudentCourses)
            //    //.ThenInclude(sc => sc.Course)
            //    //.ThenInclude(c => c.Assignments)
            //    .Where(s => s.StudentCourses.Any(sc => sc.Course.Assignments.Any(a => a.AssignmentId == assignmentId)))
            //    .Include(s => s.StudentAssignments)
            //    .ToListAsync();
                
            var studentsWithStudentAssignment = await _context.Students
                .Where(s => s.StudentCourses.Any(sc => sc.Course.Assignments.Any(a => a.AssignmentId == assignmentId)))
                .Include(s => s.StudentAssignments)
                .Select(s => new DtoStudentSubmissionResponse
                {
                    StudentId = s.StudentId,
                    FullName = s.FullName,
                    StudentAssignment = s.StudentAssignments
                    .Where(sa => sa.AssignmentId == assignmentId && sa.StudentId == s.StudentId)
                    .Select(sa => new DtoStudentAssignmentDetails
                    {
                        StudentAssignmentId = sa.StudentAssignmentId,
                        SubmissionDate = sa.SubmissionDate,
                        Grade = sa.Grade,
                        Feedback = sa.Feedback
                    }).FirstOrDefault()
                }).ToListAsync();


            //var allStudents = new List<DtoStudentSubmissionResponse>();
            //foreach (var s in studentsWithStudentAssignment)
            //{
            //    allStudents.Add(new DtoStudentSubmissionResponse
            //    {
            //        StudentId = s.StudentId,
            //        FullName = s.FullName,
            //        StudentAssignment = await _GetStudentAssignment(s.StudentId, assignmentId)
            //    });
            //}
            //return allStudents;
            return studentsWithStudentAssignment;
        }
        private async Task<DtoStudentAssignmentDetails> _GetStudentAssignment(int studentId, int assignmentId)
        {
            var studentAssignment = await _context.StudentAssignments
                .FirstOrDefaultAsync(sc => sc.AssignmentId == assignmentId && sc.StudentId == studentId);
            if (studentAssignment != null)
                return new DtoStudentAssignmentDetails
                {
                    StudentAssignmentId = studentAssignment.StudentAssignmentId,
                    SubmissionDate = studentAssignment.SubmissionDate,
                    Grade = studentAssignment.Grade,
                    Feedback = studentAssignment.Feedback
                };
            return null;
        }

    }
}
