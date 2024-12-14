
using global::SCMS_back_end.Models;
using global::SCMS_back_end.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using SCMS_back_end.Models.Dto;
using SCMS_back_end.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SCMS_back_end.Data;
using SCMS_back_end.Models.Dto.Request;

namespace SCMS_back_end.Repositories.Services
{
    public class LectureService : ILecture
    {
        private readonly StudyCenterDbContext _context;

        public LectureService(StudyCenterDbContext context)
        {
            _context = context;
        }

        public async Task AddLecturesAsync(int courseId)
        {
            // Fetch course and its schedule along with the ScheduleDays and WeekDays
            var course = await _context.Courses
                .Include(c => c.Schedule)
                .ThenInclude(s => s.ScheduleDays)
                .ThenInclude(sd => sd.WeekDay)
                .FirstOrDefaultAsync(c => c.CourseId == courseId);

            if (course == null || course.Schedule == null)
                throw new ArgumentException("Course or Schedule not found.");

            // Get the list of weekdays (e.g., Monday, Tuesday) for which lectures should be scheduled
            var weekDays = course.Schedule.ScheduleDays.Select(sd => sd.WeekDay.Name).ToList();

            // Generate lectures for the given weekdays within the date range of the schedule
            var lectures = GenerateLectures(course.Schedule.StartDate, course.Schedule.EndDate, weekDays, course.Schedule.StartTime);

            // Add generated lectures to the course
            foreach (var lecture in lectures)
            {
                lecture.CourseId = courseId;
                _context.Lectures.Add(lecture);
            }

            // Save changes to the database
            await _context.SaveChangesAsync();
        }

        private IEnumerable<Lecture> GenerateLectures(DateTime startDate, DateTime endDate, List<string> weekDays, TimeSpan startTime)
        {
            var lectures = new List<Lecture>();
            var currentDate = startDate;

            // Loop through each day in the range from startDate to endDate
            while (currentDate <= endDate)
            {
                // Check if the current day matches any of the provided weekdays
                if (weekDays.Contains(currentDate.DayOfWeek.ToString()))
                {
                    lectures.Add(new Lecture
                    {
                        LectureDate = currentDate.Date.Add(startTime), // Set the lecture date and time
                    });
                }

                // Move to the next day
                currentDate = currentDate.AddDays(1);
            }

            return lectures;
        }



        //public async Task DeleteCourseAttendanceRecordsAsync(int courseId)
        //{
        //    var course = await _context.Courses
        //        .Include(c => c.Schedule)
        //        .FirstOrDefaultAsync(c => c.CourseId == courseId);

        //    if (course == null || course.Schedule == null)
        //        throw new ArgumentException("Course or Schedule not found.");

        //    if (DateTime.Now > course.Schedule.EndDate)
        //    {
        //        var lecturesToDelete = await _context.Lectures
        //            .Where(l => l.CourseId == courseId)
        //            .ToListAsync();

        //        _context.Lectures.RemoveRange(lecturesToDelete);

        //        var attendanceToDelete = await _context.LectureAttendances
        //            .Where(a => lecturesToDelete.Select(l => l.LectureId).Contains(a.LectureId))
        //            .ToListAsync();

        //        _context.LectureAttendances.RemoveRange(attendanceToDelete);

        //        await _context.SaveChangesAsync();
        //    }
        //}
    }
}
