using Azure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Newtonsoft.Json;
using SCMS_back_end.Data;
using SCMS_back_end.Models;
using SCMS_back_end.Models.Dto.Request.LectureAttendance;
using SCMS_back_end.Models.Dto.Response.LectureAttendance;
using SCMS_back_end.Repositories.Interfaces;
using System.Globalization;
using System.Security.Claims;

namespace SCMS_back_end.Repositories.Services
{
    public class LectureAttendanceService : ILectureAttendance
    {
        private readonly StudyCenterDbContext _context;

        public LectureAttendanceService(StudyCenterDbContext context)
        {
            _context = context;
        }

        public async Task<List<LectureAttendance>> AddLectureAttendance(AddLectureAttendanceReqDto attendance)
        {

            var lecture = await _context.Lectures
                                .FirstOrDefaultAsync(l => l.LectureId == attendance.LectureId);

            if (lecture == null)
            {
                throw new Exception("Course does not exist.");
            }
            if(_context.LectureAttendances.Any(l=>l.LectureId == lecture.LectureId))
            {
                throw new Exception("Attendance for this lecture has already been posted.");
            }
            var LectureAttendance = new List<LectureAttendance>();
            foreach (var a in attendance.LectureAttendance)
            {
                LectureAttendance.Add(new LectureAttendance
                {
                    LectureId= attendance.LectureId,
                    StudentId= a.StudentId,
                    Status= a.Status
                });
            }
            //var LectureAttendance = new LectureAttendance()
            //{
            //    LectureId = attendance.LectureId,
            //    StudentId = attendance.StudentId,
            //    Status = attendance.Status,
            //};

            _context.LectureAttendances.AddRange(LectureAttendance);
            await _context.SaveChangesAsync();


            //var Response = new DtoAddLectureAttendanceResponse()
            //{
            //    LectureId = attendance.LectureId,
            //    StudentId = attendance.StudentId,
            //    Status = attendance.Status,
            //};

            return LectureAttendance;
        }
        public async Task<List<DtoGetAbsenceRateAndStudentResponse>> GetStudentAndAbsenceRate(int courseId)
        {
            var courseExists = await _context.Courses.AnyAsync(c => c.CourseId == courseId);
            if (!courseExists)
            {
                throw new Exception("Course does not exist.");
            }


            var totalLectures = await _context.Lectures
                .Where(x => x.CourseId == courseId)
                .CountAsync();


            if (totalLectures == 0)
            {
                throw new Exception("No lectures found for the given course.");
            }


            var studentAbsenceDataQuery = _context.LectureAttendances
                .Where(x => x.Lecture.CourseId == courseId)
                .Include(x => x.Student)
                .GroupBy(x => new { x.Student.StudentId, x.Student.FullName })
                .Select(g => new
                {
                    StudentID = g.Key.StudentId,
                    StudentName = g.Key.FullName,
                    AbsentCount = g.Count(x => x.Status == "Absence"),
                    AbsenceRate = totalLectures > 0
                        ? ((double)g.Count(x => x.Status == "Absence") / totalLectures) * 100
                        : 0
                });


            var studentAbsenceData = await studentAbsenceDataQuery.ToListAsync();


            //if (!studentAbsenceData.Any())
            //{
            //    throw new Exception("No attendance data found for students in the given course.");
            //}


            var responses = studentAbsenceData.Select(data => new DtoGetAbsenceRateAndStudentResponse
            {
                StudentID = data.StudentID,
                FullName = data.StudentName,
                AbsenceRateForTheStudent = Math.Round(data.AbsenceRate, 2),
            }).ToList();

            return responses;
        }
        public async Task<List<DtoGetAbsenceDateResponse>> GetAbsenceDate(int CourseId, ClaimsPrincipal userPrincipal)
        {
            var userIdClaim = userPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userIdClaim);
            if (student == null)
            {
                throw new InvalidOperationException("Student not found.");
            }

            var courseExists = await _context.Courses.AnyAsync(c => c.CourseId == CourseId);
            if (!courseExists)
            {
                throw new Exception("Course does not exist.");
            }
            
            //var studentExists = await _context.Students.AnyAsync(s => s.StudentId == student.StudentId);
            //if (!studentExists)
            //{
            //    throw new Exception("Student does not exist.");
            //}
          
            var dates = await _context.LectureAttendances
                .Where(x => x.StudentId == student.StudentId && x.Lecture.CourseId == CourseId && x.Status=="Absence")
                .Include(x => x.Lecture)
                .Select(x => x.Lecture.LectureDate)
                .ToListAsync();

            
            //if (!dates.Any())
            //{
            //    throw new Exception("No absences found for the given student in this course.");
            //}
           
            var responses = dates.Select(date => new DtoGetAbsenceDateResponse
            {
                LectureDate = date
            }).ToList();

            return responses;
        }
        public async Task<List<Lecture>> GetCourseLectures(int courseId)
        {
            var courseExists = await _context.Courses.AnyAsync(c => c.CourseId == courseId);
            if (!courseExists)
            {
                throw new Exception("Course does not exist.");
            }

            var lectures = await _context.Lectures
                .Where(x => x.CourseId == courseId)
                .ToListAsync();

            return lectures;
        }

        public async Task<List<DtoLectursResponse>> GetLectures(int CourseId)
        {
            var courseExists = await _context.Courses.AnyAsync(c => c.CourseId == CourseId);
            if (!courseExists)
            {
                throw new Exception("Course does not exist.");
            }

            var lectures = await _context.Lectures
                .Where(x => x.CourseId == CourseId)
                .Select(x => new DtoLectursResponse
                {
                    Id = x.LectureId,
                    LectureTime = x.LectureDate,
                })
                .ToListAsync();

            return lectures;
        }
    }
}
