using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SCMS_back_end.Data;
using SCMS_back_end.Models;
using SCMS_back_end.Models.Dto.Request;
using SCMS_back_end.Models.Dto.Response;
using SCMS_back_end.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SCMS_back_end.Repositories.Services
{
    public class StudentService : IStudent
    {
        private readonly StudyCenterDbContext _context;

        public StudentService(StudyCenterDbContext context)
        {
            _context = context;
        }

        //public async Task DeleteStudentAsync(int id)
        //{
        //    // Retrieve the student from the database, including their enrolled courses
        //    var student = await _context.Students
        //                                .Include(s => s.StudentCourses)
        //                                .FirstOrDefaultAsync(s => s.StudentId == id);

        //    if (student == null)
        //    {
        //        throw new KeyNotFoundException("Student not found.");
        //    }

        //    // Check if the student is enrolled in any courses
        //    if (student.StudentCourses.Any())
        //    {
        //        throw new InvalidOperationException("Cannot delete a student who is currently enrolled in a course.");
        //    }

        //    // Remove the student from the database
        //    _context.Students.Remove(student);
        //    await _context.SaveChangesAsync();
        //}
        public async Task DropStudentFromCourseAsync(int studentId, int courseId)
        {
            // Fetch the student
            var student = await _context.Students
                .Include(s => s.StudentCourses)
                .SingleOrDefaultAsync(s => s.StudentId == studentId);

            if (student == null)
            {
                throw new KeyNotFoundException($"Student with ID {studentId} not found.");
            }

            // Fetch the course
            var course = await _context.Courses
                .SingleOrDefaultAsync(c => c.CourseId == courseId);

            if (course == null)
            {
                throw new KeyNotFoundException($"Course with ID {courseId} not found.");
            }

            // Find the enrollment record
            var enrollment = student.StudentCourses
                .SingleOrDefault(sc => sc.CourseId == courseId);

            if (enrollment == null)
            {
                throw new InvalidOperationException($"Student with ID {studentId} is not enrolled in course {courseId}.");
            }

            // Update status to "Dropped"
            enrollment.Status = "Drop";

            _context.StudentCourses.Update(enrollment);
            await _context.SaveChangesAsync();
        }
        public async Task EnrollStudentInCourseAsync(ClaimsPrincipal userPrincipal, int courseId)
        {
            var userIdClaim = userPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userIdClaim);
            if (student == null)
            {
                throw new InvalidOperationException("Student not found.");
            }
            // Fetch the student
             student = await _context.Students
                .Include(s => s.StudentCourses)
                .ThenInclude(sc => sc.Course)
                .ThenInclude(c => c.Schedule)
                .ThenInclude(s => s.ScheduleDays)
                .SingleOrDefaultAsync(s => s.StudentId == student.StudentId);

            if (student == null)
            {
                throw new KeyNotFoundException($"Student with ID {student.StudentId} not found.");
            }

            // Fetch the course
            var course = await _context.Courses
                .Include(c => c.StudentCourses)
                .Include(c => c.Schedule)
                .ThenInclude(s => s.ScheduleDays)
                .SingleOrDefaultAsync(c => c.CourseId == courseId);

            if (course == null)
            {
                throw new KeyNotFoundException($"Course with ID {courseId} not found.");
            }

            // Check if the student is already enrolled in this course
            var existingEnrollment = student.StudentCourses
                .SingleOrDefault(sc => sc.CourseId == courseId);

            if (existingEnrollment != null)
            {
                throw new InvalidOperationException($"Student with ID {student.StudentId} is already enrolled in course {courseId}.");
            }

            // Check course capacity
            if (course.StudentCourses.Count >= course.Capacity)
            {
                throw new InvalidOperationException($"Course with ID {courseId} is at full capacity.");
            }

            //// Check if student level is appropriate
            //if (student.Level > course.Level || student.Level < course.Level)
            //{
            //    throw new InvalidOperationException($"Student level ({student.Level}) is not equal the course level ({course.Level}).");
            //}

            var studentCourses = await _context.Courses
                .Include(c => c.StudentCourses)
                .Include(c => c.Schedule)
                .ThenInclude(s => s.ScheduleDays)
                .Where(c => c.StudentCourses.Any(sc => sc.StudentId == student.StudentId) && c.Schedule.StartDate < course.Schedule.EndDate &&
                c.Schedule.EndDate > course.Schedule.StartDate).ToListAsync();

            foreach (var teacherCourse in studentCourses)
            {
                if (teacherCourse.CourseId != courseId && teacherCourse.Schedule != null)
                {

                    // check if there is an overlap between days 
                    var course1Days = teacherCourse.Schedule.ScheduleDays.Select(sd => sd.WeekDayId).ToList();
                    var course2Days = course.Schedule.ScheduleDays.Select(sd => sd.WeekDayId).ToList();
                    var commonDays = course1Days.Intersect(course2Days).ToList();

                    if (commonDays.Any())
                    {
                        // check if there is an overlap between times 
                        var overlap = teacherCourse.Schedule.StartTime < course.Schedule.EndTime &&
                             teacherCourse.Schedule.EndTime > course.Schedule.StartTime;

                        if (overlap)
                        {
                            throw new Exception("Student has a conflicting course schedule.");
                        }
                    }
                }
            }

            // Add new enrollment
            var newEnrollment = new StudentCourse
            {
                StudentId = student.StudentId,
                CourseId = courseId,
                EnrollmentDate = DateTime.Now,
                Status = "Enrolled"
            };

            _context.StudentCourses.Add(newEnrollment);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<StudentDtoResponse>> GetAllStudentsAsync()
        {
            // Fetch students from the database
            var students = await _context.Students
                .Include(s => s.StudentCourses)
                .Include(s => s.StudentAssignments)
                .Include(s => s.LectureAttendances)
                .ToListAsync();

            // Map students to StudentDtoResponse
            var studentDtos = students.Select(s => new StudentDtoResponse
            {
                StudentId = s.StudentId.ToString(),  // Convert int to string if needed
                FullName = s.FullName,
                PhoneNumber = s.PhoneNumber
            }).ToList();

            return studentDtos;
        }
        public async Task<StudentDtoResponse> GetStudentByIdAsync(int id)
        {
            var student = await _context.Students
                .Include(s => s.StudentCourses)
                .Include(s => s.StudentAssignments)
                .Include(s => s.LectureAttendances)
                .FirstOrDefaultAsync(s => s.StudentId == id);

            if (student == null)
            {
                throw new KeyNotFoundException($"Student with ID {id} not found.");
            }

            return new StudentDtoResponse
            {
                StudentId = student.StudentId.ToString(),  // Assuming StudentId is an int, convert to string if needed
                FullName = student.FullName,
                PhoneNumber = student.PhoneNumber
            };
        }
        public async Task<IEnumerable<StudentDtoResponse>> GetStudentsByCourseIdAsync(int courseId)
        {
            // Fetch students who are enrolled in the specified course and are not dropped
            // To include related data if necessary
            var students = await _context.StudentCourses
                .Where(sc => sc.CourseId == courseId && sc.Status != "Drop")
                .Select(sc => sc.Student)
                .ToListAsync();

            // Convert to StudentDtoResponse
            var studentDtos = students.Select(s => new StudentDtoResponse
            {
                StudentId = s.StudentId.ToString(),
                FullName = s.FullName,
                PhoneNumber = s.PhoneNumber
            });

            return studentDtos;
        }
        public async Task UpdateStudentAsync(int id, StudentDtoRequest studentDto)
        {
            if (_context.Students == null)
            {
                throw new InvalidOperationException("Entity set 'StudyCenterDbContext.Students' is null.");
            }

            if (studentDto == null)
            {
                throw new ArgumentNullException(nameof(studentDto), "Student data is required.");
            }

            var existingStudent = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == id);

            if (existingStudent == null)
            {
                throw new KeyNotFoundException($"Student with ID {id} was not found.");
            }

            // Update FullName
            if (!string.IsNullOrEmpty(studentDto.FullName))
                existingStudent.FullName = studentDto.FullName;

            // Update PhoneNumber if provided
            if (!string.IsNullOrWhiteSpace(studentDto.PhoneNumber))
            {
                existingStudent.PhoneNumber = studentDto.PhoneNumber;
            }

           /* // Update Level only if the new level is greater than the current level
            if (studentDto.Level != 0)
            {
                if (studentDto.Level > existingStudent.Level)
                {
                    existingStudent.Level = studentDto.Level;
                }
                else
                {
                    throw new ArgumentException("New level must be higher than the current level.");
                }
            }*/


            _context.Students.Update(existingStudent);
            await _context.SaveChangesAsync();
        }
    }
}
