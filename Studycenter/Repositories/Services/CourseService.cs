using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using SCMS_back_end.Data;
using SCMS_back_end.Models;
using SCMS_back_end.Models.Dto.Grades;
using SCMS_back_end.Models.Dto.Request;
using SCMS_back_end.Models.Dto.Response;
using SCMS_back_end.Repositories.Interfaces;
using SCMS_back_end.Services;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace SCMS_back_end.Repositories.Services
{
    public class CourseService : ICourse
    {
        private readonly StudyCenterDbContext _context;

        public CourseService(StudyCenterDbContext context)
        {
            _context = context;
        }
        public async Task CalculateAverageGrade(int courseId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
            {
                throw new Exception("Course not found");
            }
            var studentCourses = await _context.StudentCourses.Where(sc => sc.CourseId == courseId).ToListAsync();
            foreach (var studentCourse in studentCourses)
            {
                await CalculateStudentGrade(courseId, studentCourse.StudentId);
            }
        }
        public async Task<Course> CreateCourseWithoutTeacher(DtoCreateCourseWTRequest courseRequest)
        {
            // Check if the classroom exists
            var classroom = await _context.Classrooms.FindAsync(courseRequest.ClassroomId);
            if (classroom == null)
            {
                throw new Exception("Classroom not found");
            }

            // Check if the course capacity is less than or equal to the classroom capacity
            if (courseRequest.Capacity > classroom.Capacity)
            {
                throw new Exception("Course capacity exceeds classroom capacity");
            }

            // Check if the classroom is available
            var overlappingCourses = await _context.Courses
                .Include(c => c.Schedule)
                .ThenInclude(s => s.ScheduleDays)
                .Where(c => c.ClassroomId == courseRequest.ClassroomId &&
                            c.Schedule.StartDate < courseRequest.EndDate &&
                            c.Schedule.EndDate > courseRequest.StartDate &&
                            c.Schedule.ScheduleDays.Any(sd => courseRequest.WeekDays.Contains(sd.WeekDayId)) &&
                            c.Schedule.StartTime < courseRequest.EndTime &&
                            c.Schedule.EndTime > courseRequest.StartTime)
                .ToListAsync();

            if (overlappingCourses.Any())
            {
                throw new Exception("Classroom is not available at the specified time");
            }

            // Create the schedule
            var schedule = new Schedule
            {
                StartDate = courseRequest.StartDate,
                EndDate = courseRequest.EndDate,
                StartTime = courseRequest.StartTime,
                EndTime = courseRequest.EndTime,
                ScheduleDays = courseRequest.WeekDays.Select(id => new ScheduleDay { WeekDayId = id }).ToList()
            };

            await _context.Schedules.AddAsync(schedule);
            await _context.SaveChangesAsync();

            // Create the course
            var course = new Course
            {
                SubjectId = courseRequest.SubjectId,
                ClassName = courseRequest.ClassName,
                Capacity = courseRequest.Capacity,
                ScheduleId = schedule.ScheduleId,
                ClassroomId = courseRequest.ClassroomId,
                Price = courseRequest.Price,
                Description = courseRequest.Description,
                Level = courseRequest.Level,
                ImageUrl = courseRequest.ImageUrl
            };

            await _context.Courses.AddAsync(course);
            await _context.SaveChangesAsync();

            // Add lectures
            var lectureService = new LectureService(_context);
            await lectureService.AddLecturesAsync(course.CourseId);

            // Add NumberOfHours
            await CalculateCourseHours(course);

            return course;
        }
        public async Task DeleteCourse(int courseId)
        {
            var course = await _context.Courses
                .Include(c => c.StudentCourses)
                .FirstOrDefaultAsync(c => c.CourseId == courseId);
            var scheduleDays = await _context.ScheduleDays
                .Include(sd => sd.Schedule)
                .ThenInclude(s => s.Course)
                .Where(sd => sd.Schedule.Course.CourseId == courseId)
                .ToListAsync();
            var schedule = await _context.Schedules
                .Include(s => s.Course)
                .FirstOrDefaultAsync(s => s.Course.CourseId == courseId);
               


            if (course == null) return;

            if(course.StudentCourses.Any())
            {
                throw new InvalidOperationException($"Cannot delete course {courseId}: students are enrolled.");
            }
            _context.ScheduleDays.RemoveRange(scheduleDays);
            _context.Schedules.Remove(schedule);
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
        }
        public async Task<List<DtoCourseResponse>> GetAllCourses()
        {
            var courses = await _context.Courses.ToListAsync();
            var courseResponse = new List<DtoCourseResponse>();
            foreach (var course in courses)
            {
                var courseRes = await GetCourseById(course.CourseId);
                courseResponse.Add(courseRes);
            }
            return courseResponse;
        }
        public async Task<DtoCourseResponse> GetCourseById(int courseId)
        {
            var course = await _context.Courses
        .Include(c => c.Teacher)
        .ThenInclude(t => t.Department)
        .Include(c => c.Subject)
        .Include(c => c.Schedule)
        .ThenInclude(s => s.ScheduleDays)
        .ThenInclude(sd => sd.WeekDay)
        .Include(c => c.Classroom)
        .FirstOrDefaultAsync(c => c.CourseId == courseId);

            if (course == null)
            {
                throw new Exception("Course not found");
            }

            var courseDays = course.Schedule.ScheduleDays
                .Select(sd => sd.WeekDay.Name)
                .ToList();

            var courseResponse = new DtoCourseResponse
            {
                CourseId = courseId,
                TeacherName = course.Teacher?.FullName,
                TeacherId = course.TeacherId,
                TeacherDepartment = course.Teacher?.Department?.Name,
                SubjectName = course.Subject?.Name,
                StartDate = course.Schedule.StartDate,
                EndDate = course.Schedule.EndDate,
                StartTime = course.Schedule.StartTime,
                EndTime = course.Schedule.EndTime,
                Days = courseDays,
                ClassName = course.ClassName,
                Capacity = course.Capacity,
                ClassroomNumber = course.Classroom?.RoomNumber ?? "", // Include classroom name
                IsComplete = course.IsCompleted,
                Marks = course.Mark,
            };

            return courseResponse;
        }
        public async Task<List<DtoCourseResponse>> GetCoursesNotStarted(ClaimsPrincipal userPrincipal)
        {
            var userIdClaim = userPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userIdClaim);
            if (student == null)
            {
                throw new InvalidOperationException("Student not found.");
            }

            var courses = await _context.Courses.Include(c => c.Teacher)
                                                .Include (c => c.Classroom)
                                                .Include(c => c.Subject)
                                                .ThenInclude(s=>s.Department)
                                                .Include(c => c.StudentCourses)
                                                .Include(c => c.Schedule)
                                                .ThenInclude(s => s.ScheduleDays)
                                                .ThenInclude(sd => sd.WeekDay)
                                                .Where(c => c.Schedule.StartDate > DateTime.Now 
                                                && c.StudentCourses.All(sc=>sc.StudentId != student.StudentId))
                                                .ToListAsync();

            var courseResponse = new List<DtoCourseResponse>();
            foreach (var course in courses)
            {
                var courseDays = course.Schedule.ScheduleDays
                    .Select(sd => sd.WeekDay.Name)
                    .ToList();

                var courseRes = new DtoCourseResponse
                {
                    CourseId = course.CourseId,
                    ClassName = course.ClassName,
                    TeacherName = course.Teacher?.FullName ?? "N/A",
                    Department = course.Subject?.Department?.Name ?? "N/A",
                    SubjectName = course.Subject?.Name ?? "N/A",
                    StartDate = course.Schedule.StartDate,
                    EndDate = course.Schedule.EndDate,
                    StartTime = course.Schedule.StartTime,
                    EndTime = course.Schedule.EndTime,
                    Days = courseDays,
                    Description= course.Description ?? "",
                    Level = course.Level,
                    Capacity = course.Capacity,
                    NumberOfEnrolledStudents=course.StudentCourses.Count,
                    Price= course.Price,
                    NumberOfHours= course.NumberOfHours,
                };

                courseResponse.Add(courseRes);
            }
            return courseResponse;
        }
        public async Task<List<DtoCourseResponse>> GetCoursesOfStudent(ClaimsPrincipal userPrincipal)
        {
            var userIdClaim = userPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userIdClaim);
            if (student == null)
            {
                throw new Exception("Student not found");
            }
            var courses = await _context.Courses.Include(c => c.Teacher)
                                                .Include(c => c.Classroom)
                                                .Include(c => c.Subject)
                                                .Include(c => c.Schedule)
                                                .ThenInclude(s => s.ScheduleDays)
                                                .ThenInclude(sd => sd.WeekDay)
                                                .Include(c=>c.StudentCourses)
                                                .ThenInclude(sc=>sc.Certificate)
                                                .Where(c => c.StudentCourses.Any(s => s.StudentId == student.StudentId))
                                                .ToListAsync();
            var courseResponses = new List<DtoCourseResponse>();
            foreach (var course in courses)
            {
                var courseDays = course.Schedule.ScheduleDays
                    .Select(sd => sd.WeekDay.Name)
                    .ToList();
                var studentCourse= course.StudentCourses.FirstOrDefault(sc => sc.StudentId == student.StudentId);
                var courseRes = new DtoCourseResponse
                {
                    CourseId = course.CourseId,
                    TeacherName = course.Teacher?.FullName ?? "N/A",
                    SubjectName = course.Subject?.Name ?? "N/A",
                    StartDate = course.Schedule.StartDate,
                    EndDate = course.Schedule.EndDate,
                    StartTime = course.Schedule.StartTime,
                    EndTime = course.Schedule.EndTime,
                    Days = courseDays,
                    ClassName = course.ClassName,
                    Capacity = course.Capacity,
                    NumberOfHours = course.NumberOfHours,
                    CertificateId = studentCourse?.Certificate?.CertificateId ?? "",
                    AverageGrades = studentCourse?.AverageGrades ?? 0,
                    ClassroomNumber= course.Classroom.RoomNumber,
                    IsComplete= course.IsCompleted,
                };

                courseResponses.Add(courseRes);
            }
            return courseResponses;
        }
        public async Task<List<DtoCourseResponse>> GetCoursesOfTeacher(ClaimsPrincipal userPrincipal)
        {
            var userIdClaim = userPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.UserId == userIdClaim);
            if (teacher == null)
            {
                throw new Exception("Teacher not found");
            }
            var courses = await _context.Courses.Where(c => c.TeacherId == teacher.TeacherId).ToListAsync();
            var courseResponse = new List<DtoCourseResponse>();
            foreach (var course in courses)
            {
                var courseRes = await GetCourseById(course.CourseId);
                courseResponse.Add(courseRes);
            }
            return courseResponse;
        }

        public async Task<List<DtoCourseResponse>> GetCurrentCoursesOfStudent(ClaimsPrincipal userPrincipal)
        {
            var userIdClaim = userPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userIdClaim);
            if (student == null)
            {
                throw new Exception("Student not found");
            }
            var currentCourses = await _context.Courses.Include(c => c.Teacher)
                                                       .Include(c => c.Classroom)
                                                       .Include(c => c.Subject)
                                                       .Include(c => c.Schedule)
                                                       .ThenInclude(s => s.ScheduleDays)
                                                       .ThenInclude(sd => sd.WeekDay)
                                                       .Where(c => c.StudentCourses.Any(s => s.StudentId == student.StudentId) &&  c.Schedule.EndDate >= DateTime.Now)
                                                       .ToListAsync();

            var currentCourseResponses = new List<DtoCourseResponse>();
            foreach (var course in currentCourses)
            {
                var courseDays = course.Schedule.ScheduleDays
                    .Select(sd => sd.WeekDay.Name)
                    .ToList();

                var courseRes = new DtoCourseResponse
                {
                    CourseId = course.CourseId,
                    TeacherName = course.Teacher?.FullName ?? "N/A",
                    SubjectName = course.Subject?.Name ?? "N/A",
                    StartDate = course.Schedule.StartDate,
                    EndDate = course.Schedule.EndDate,
                    StartTime = course.Schedule.StartTime,
                    EndTime = course.Schedule.EndTime,
                    Days = courseDays,
                    ClassName = course.ClassName,
                    Capacity = course.Capacity,
                    ClassroomNumber= course.Classroom?.RoomNumber ?? "N/A"
                };

                currentCourseResponses.Add(courseRes);
            }
            return currentCourseResponses;
        }
        public async Task<List<DtoCourseResponse>> GetCurrentCoursesOfTeacher(ClaimsPrincipal userPrincipal)
        {
            var userIdClaim = userPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.UserId == userIdClaim);
            if (teacher == null)
            {
                throw new Exception("Teacher not found");
            }
            var currentCourses = await _context.Courses.Include(c => c.Teacher)
                                                       .Include(c => c.Subject)
                                                       .Include(c=>c.Classroom)
                                                       .Include(c => c.Schedule)
                                                       .ThenInclude(s => s.ScheduleDays)
                                                       .ThenInclude(sd => sd.WeekDay)
                                                       .Where(c => c.TeacherId == teacher.TeacherId &&  c.IsCompleted)
                                                       .ToListAsync();


            var currentCourseResponses = new List<DtoCourseResponse>();
            foreach (var course in currentCourses)
            {
                var courseDays = course.Schedule.ScheduleDays
                    .Select(sd => sd.WeekDay.Name)
                    .ToList();

                var courseRes = new DtoCourseResponse
                {
                    TeacherName = course.Teacher?.FullName ?? "N/A",
                    SubjectName = course.Subject?.Name ?? "N/A",
                    StartDate = course.Schedule.StartDate,
                    EndDate = course.Schedule.EndDate,
                    StartTime = course.Schedule.StartTime,
                    EndTime = course.Schedule.EndTime,
                    Days = courseDays,
                    ClassName = course.ClassName,
                    Capacity = course.Capacity,
                    ClassroomNumber = course.Classroom?.RoomNumber ?? "N/A",
                };

                currentCourseResponses.Add(courseRes);
            }
            return currentCourseResponses;
        }
        public async Task<List<DtoPreviousCourseResponse>> GetPreviousCoursesOfStudent(ClaimsPrincipal userPrincipal)
        {
            var userIdClaim = userPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userIdClaim);
            if (student == null)
            {
                throw new Exception("Student not found");
            }
            var previousCourses = await _context.Courses.Include(c => c.Teacher)
                                                        .Include(c => c.Subject)
                                                        .Include(c => c.Schedule)
                                                        .ThenInclude(s => s.ScheduleDays)
                                                        .ThenInclude(sd => sd.WeekDay)
                                                        .Where(c => c.StudentCourses.Any(s => s.StudentId == student.StudentId) && c.Schedule.EndDate < DateTime.Now)
                                                        .ToListAsync();

            var previousCourseResponses = new List<DtoPreviousCourseResponse>();
            foreach (var course in previousCourses)
            {
                var courseDays = course.Schedule.ScheduleDays
                    .Select(sd => sd.WeekDay.Name)
                    .ToList();

                var courseRes = new DtoPreviousCourseResponse
                {
                    CourseId = course.CourseId,
                    TeacherName = course.Teacher?.FullName ?? "N/A",
                    SubjectName = course.Subject?.Name ?? "N/A",
                    CourseName = course.ClassName,
                    Grade = course.StudentCourses.FirstOrDefault(sc => sc.StudentId == student.StudentId)?.AverageGrades ?? 0,
                    Status = course.StudentCourses.FirstOrDefault(sc => sc.StudentId == student.StudentId)?.Status ?? "N/A",
                    StartDate = course.Schedule.StartDate,
                    EndDate = course.Schedule.EndDate,
                    StartTime = course.Schedule.StartTime,
                    EndTime = course.Schedule.EndTime,
                };

                previousCourseResponses.Add(courseRes);
            }
            return previousCourseResponses;
        }
        public async Task CalculateStudentGrade(int courseId, int studentId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
            {
                throw new Exception("Course not found");
            }
            var student = await _context.Students.FindAsync(studentId);
            if (student == null)
            {
                throw new Exception("Student not found");
            }
            var studentCourse = await _context.StudentCourses.Where(sc => sc.CourseId == courseId && sc.StudentId == studentId).FirstOrDefaultAsync();
            if (studentCourse == null)
            {
                throw new Exception("Student is not enrolled in this course");
            }
            var studentGrades = await _context.StudentAssignments.Where(sa => sa.StudentId == studentId).ToListAsync();
            var courseAssignments = await _context.Assignments.Where(a => a.CourseId == courseId).ToListAsync();
            var courseGrades = studentGrades.Where(sg => courseAssignments.Select(ca => ca.AssignmentId).Contains(sg.AssignmentId)).ToList();
            int sum = 0;
            foreach (var grade in courseGrades)
            {
                sum += grade.Grade ?? 0;
            }
            double average = sum / courseGrades.Count;
            studentCourse.AverageGrades = (int)average;
            if (studentCourse.Status != "Drop")
                studentCourse.Status = average >= 50 ? "Pass" : "Fail";
            await _context.SaveChangesAsync();
        }
        public async Task<Course> UpdateCourseInformation(int courseId, DtoUpdateCourseRequest courseRequest)
        {
            var course = await _context.Courses
                .Include(c => c.Subject)
                .Include(c => c.StudentCourses)
                .Include(c => c.Schedule)
                .ThenInclude(s => s.ScheduleDays)
                .FirstOrDefaultAsync(c => c.CourseId == courseId);
            

            if (course == null)
            {
                throw new Exception("Course not found");
            }

            //if (courseRequest.SubjectId != 0)
            //{
            //    var subjectExists = await _context.Subjects.AnyAsync(s => s.SubjectId == courseRequest.SubjectId);
            //    if (!subjectExists)
            //    {
            //        throw new Exception("Subject not found");
            //    }
            //    course.SubjectId = courseRequest.SubjectId;
            //}

            //if (courseRequest.Level != 0)
            //{
            //    course.Level = courseRequest.Level;
            //}
            if (courseRequest.Capacity != 0)
            {
                if (courseRequest.Capacity < course.StudentCourses.Count)
                    throw new Exception("Capacity cannot be decreased below the number of registered students.");
                course.Capacity = courseRequest.Capacity;
            }

            if (courseRequest.TeacherId.HasValue /*&& courseRequest.TeacherId!=0*/)
            {
                var teacher = await _context.Teachers
                    .Include(t => t.Courses)
                    .ThenInclude(c => c.Schedule)
                    .ThenInclude(s => s.ScheduleDays)
                    .FirstOrDefaultAsync(t => t.TeacherId == courseRequest.TeacherId.Value);

                var TeacherCourses = await _context.Courses
                .Include(c => c.Subject)
                .Include(c => c.Schedule)
                .ThenInclude(s => s.ScheduleDays)
                .Where(c => c.TeacherId == courseRequest.TeacherId.Value && c.Schedule.StartDate < course.Schedule.EndDate &&
                c.Schedule.EndDate > course.Schedule.StartDate).ToListAsync();


                if (teacher == null)
                {
                    throw new Exception("Teacher not found");
                }

                foreach (var teacherCourse in TeacherCourses)
                {
                    if (teacherCourse.CourseId != courseId && teacherCourse.Schedule != null)
                    {
                        // check if there is an overlap between dates 
                        //if (teacherCourse.Schedule.StartDate < course.Schedule.EndDate &&
                        //    teacherCourse.Schedule.EndDate > course.Schedule.StartDate)
                        //{
                            //var overlap = teacherCourse.Schedule.ScheduleDays.Any(sd => course.Schedule.ScheduleDays.Select(cs => cs.WeekDayId).Contains(sd.WeekDayId)) &&
                            //              teacherCourse.Schedule.StartTime == course.Schedule.StartTime &&
                            //              teacherCourse.Schedule.EndTime == course.Schedule.EndTime;

                            //if (overlap)
                            //{
                            //    throw new Exception("Teacher has a conflicting course schedule.");
                            //}

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
                                    throw new Exception("Teacher has a conflicting course schedule.");
                                }
                            }

                        //}
                        
                    }
                }

                if (TeacherCourses.Count >= teacher.CourseLoad)
                {
                    throw new Exception("Teacher has reached the maximum course load.");
                }

                var courseDepartment = await _context.Departments
                    .FirstOrDefaultAsync(d => d.DepartmentId == course.Subject.DepartmentId);
                var teacherDepartment = await _context.Departments
                    .FirstOrDefaultAsync(d => d.DepartmentId == teacher.DepartmentId);

                if (courseDepartment == null || teacherDepartment == null || courseDepartment.DepartmentId != teacherDepartment.DepartmentId)
                {
                    throw new Exception("Teacher's department does not match the course department.");
                }

                course.TeacherId = courseRequest.TeacherId;
            }
            /*
            //if (courseRequest.StartDate.HasValue)
            //{
            //    course.Schedule.StartDate = courseRequest.StartDate.Value;
            //}
            //if (courseRequest.EndDate.HasValue)
            //{
            //    course.Schedule.EndDate = courseRequest.EndDate.Value;
            //}
            //if (courseRequest.StartTime.HasValue)
            //{
            //    course.Schedule.StartTime = courseRequest.StartTime.Value;
            //}
            //if (courseRequest.EndTime.HasValue)
            //{
            //    course.Schedule.EndTime = courseRequest.EndTime.Value;
            //}

            //if (courseRequest.WeekDays != null && courseRequest.WeekDays.Any())
            //{
            //    var existingScheduleDays = await _context.ScheduleDays.Where(sd => sd.ScheduleId == course.Schedule.ScheduleId).ToListAsync();
            //    _context.ScheduleDays.RemoveRange(existingScheduleDays);

            //    var newScheduleDays = courseRequest.WeekDays.Select(weekDayId => new ScheduleDay
            //    {
            //        WeekDayId = weekDayId,
            //        ScheduleId = course.Schedule.ScheduleId
            //    }).ToList();

            //    await _context.ScheduleDays.AddRangeAsync(newScheduleDays);
            //}
            */

            if (courseRequest.ClassroomId != null)
            {
                var classroom = await _context.Classrooms.FindAsync(courseRequest.ClassroomId);
                if (classroom == null)
                {
                    throw new Exception("Classroom not found");
                }

                if (courseRequest.Capacity > classroom.Capacity)
                {
                    throw new Exception("Course capacity exceeds classroom capacity");
                }

                var scheduleDays = await _context.ScheduleDays
                    .Where(sd => sd.ScheduleId == course.ScheduleId)
                    .ToListAsync();
                var overlappingCourses = await _context.Courses
                    .Include(c => c.Schedule)
                    .ThenInclude(s => s.ScheduleDays)
                    .Where(c => c.ClassroomId == courseRequest.ClassroomId &&
                                c.Schedule.StartDate < course.Schedule.EndDate &&
                                c.Schedule.EndDate > course.Schedule.StartDate &&
                                c.Schedule.ScheduleDays.Any(sd => scheduleDays.Select(s => s.WeekDayId).Contains(sd.WeekDayId)) &&
                                c.Schedule.StartTime < course.Schedule.EndTime &&
                                c.Schedule.EndTime > course.Schedule.StartTime)
                    .ToListAsync();

                if (overlappingCourses.Any())
                {
                    throw new Exception("Classroom is not available at the specified time");
                }

                course.ClassroomId = courseRequest.ClassroomId;
            }

            await _context.SaveChangesAsync();

            return course;
        }
        private async Task CalculateCourseHours(Course course)
        {
            var numOfLectures= await _context.Lectures.Where(l => l.CourseId == course.CourseId).CountAsync();
            var lectureDurationInHours= course.Schedule.EndTime.Subtract(course.Schedule.StartTime).TotalHours;
            course.NumberOfHours= numOfLectures * (decimal)lectureDurationInHours;
            await _context.SaveChangesAsync();
        }
        
        // handle grades calculation
        public async Task CalculateCourseScores(int courseId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
            {
                throw new Exception("Course not found");
            }
            var studentCourses = await _context.StudentCourses.Where(sc => sc.CourseId == courseId).ToListAsync();
            foreach (var studentCourse in studentCourses)
            {
                await CalculateStudentCourseScore(studentCourse.StudentCourseId);
            }
        }
        private async Task CalculateStudentCourseScore(int studentCourseId)
        {
            Console.WriteLine("calculate student course score is called", studentCourseId);
            var studentCourse = await _context.StudentCourses.Include(st=>st.Course).Where(st=>st.StudentCourseId == studentCourseId).FirstOrDefaultAsync();
            if (studentCourse == null)
            {
                throw new Exception("Student course not found");
            }
            var assignments= await _context.Assignments.Include(a=>a.StudentAssignments).Where(a => a.CourseId == studentCourse.CourseId).ToListAsync();
            var quizzes= await _context.Quizzes.Include(q => q.QuizResult).Where(q => q.CourseId == studentCourse.CourseId).ToListAsync();
            var courseMark = studentCourse?.Course?.Mark;
            var assignmentsScores = 0.00;
            var quizzesScores = 0.00 ;

            if (courseMark != null && courseMark != 0)
            {
                foreach (var assignment in assignments)
                {
                    var weight = (double)assignment.FullMark / courseMark * 100 ?? 0;
                    var studentAssignment = assignment.StudentAssignments.FirstOrDefault(sa => sa.StudentId == studentCourse.StudentId);
                    if (studentAssignment != null)
                    {
                        int grade = studentAssignment.Grade ?? 0;
                        int fullMark = assignment.FullMark;

                        assignmentsScores += (double)grade / fullMark * weight;

                    }
                }
                foreach (var quiz in quizzes)
                {
                    var weight = (double)quiz.Mark / courseMark * 100 ?? 0;
                    var quizResult = quiz.QuizResult.FirstOrDefault(qr => qr.StudentId == studentCourse.StudentId);
                    if (quizResult != null)
                    {
                        int grade = quizResult.Score;
                        int fullMark = quiz.Mark;

                        quizzesScores += (double)grade / fullMark * weight;

                    }
                }
                studentCourse.AverageGrades = (int)Math.Round(assignmentsScores + quizzesScores);
                studentCourse.QuizzesScore = (int)Math.Round(quizzesScores);
                studentCourse.AssignmentsScore = (int)Math.Round(assignmentsScores);
                await _context.SaveChangesAsync();
            }

        }
        public async Task CalculateCourseMark(int courseId)
        {
            var course = await _context.Courses
                .Include(c => c.Quizzes)
                .Include(c => c.Assignments)
                .FirstOrDefaultAsync(c => c.CourseId == courseId);
            if (course == null)
            {
                throw new Exception("Course not found");
            }
            var assignmentsMarks= course.Assignments.Select(a => a.FullMark).Sum();
            var quizzesMarks = course.Quizzes.Select(q => q.Mark).Sum();
            Console.WriteLine("calculate course mark is called",quizzesMarks);
            course.Mark = assignmentsMarks + quizzesMarks;
            await _context.SaveChangesAsync();
        }

        // handle get grades route
        public async  Task<List<CourseGradesResponseDTO>> GetStudentCourseGrades(int courseId)
        {
            var course = await _context.Courses.Include(c => c.StudentCourses).ThenInclude(sc => sc.Student).
                Where(c => c.CourseId == courseId).FirstOrDefaultAsync();
            if(course == null)
            {
                throw new Exception("Course not found");
            }
            var courseGrades = new List<CourseGradesResponseDTO>();
            foreach (var studentCourse in course.StudentCourses)
            {
                var item = new CourseGradesResponseDTO()
                {
                    StudentId = studentCourse.StudentId,
                    StudentName = studentCourse.Student?.FullName ?? "",
                    averageGrades= studentCourse.AverageGrades,
                    assignments= studentCourse.AssignmentsScore,
                    quizzes= studentCourse.QuizzesScore,
                };
                courseGrades.Add(item);
            }
            return courseGrades;
        }
        public async Task<StudentGradesResponseDTO> GetStudentGradesInCourse(int courseId, ClaimsPrincipal userPrincipal)
        {
            var userIdClaim = userPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userIdClaim);
            if (student == null)
            {
                throw new InvalidOperationException("Student not found.");
            }

            var course= await _context.Courses.Include(c=>c.Assignments).
                ThenInclude(a=>a.StudentAssignments).
                Include(c=>c.Quizzes).
                ThenInclude(q=>q.QuizResult).
                FirstOrDefaultAsync(c => c.CourseId == courseId);
            if (course == null)
            {
                throw new InvalidOperationException("Course not found.");
            }

            var studentCourse = await _context.StudentCourses.
                FirstOrDefaultAsync(sc => sc.CourseId == courseId && sc.StudentId == student.StudentId);
            if(studentCourse == null)
            {
                throw new InvalidOperationException("Student is not enrolled in this course.");
            }

            var assignmentsGrades = new List<AssignmentResponseDTO>();
            var quizzesGrades = new List<QuizResponseDTO>();

            foreach (var assignment in course.Assignments)
            {
                var item= new AssignmentResponseDTO
                {
                    Title= assignment.AssignmentName,
                    FullMark= assignment.FullMark,
                    AchievedMark= assignment.StudentAssignments.FirstOrDefault(sa=>sa.StudentId == student.StudentId)?.Grade ?? 0
                };
                assignmentsGrades.Add(item);
            }
            foreach (var quiz in course.Quizzes)
            {
                var item = new QuizResponseDTO
                {
                    Title = quiz.Title,
                    FullMark = quiz.Mark,
                    AchievedMark = quiz.QuizResult.FirstOrDefault(qr => qr.StudentId == student.StudentId)?.Score ?? 0
                };
                quizzesGrades.Add(item);
            }
            var studentGrades = new StudentGradesResponseDTO
            {
                OverallGrade = studentCourse.AverageGrades,
                QuizzesGrade = studentCourse.QuizzesScore,
                AssignmentsGrade = studentCourse.AssignmentsScore,
                Assignments = assignmentsGrades, 
                Quizzes = quizzesGrades
            };
            return studentGrades;
        }
       
      
    }
}
