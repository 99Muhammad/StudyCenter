using Microsoft.EntityFrameworkCore;
using SCMS_back_end.Data;
using SCMS_back_end.Repositories.Interfaces;

namespace SCMS_back_end.Repositories.Services
{
    public class WeeklyTaskService : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly IServiceScopeFactory _scopeFactory;

        public WeeklyTaskService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        // This method, a Timer is initialized to call the DoWeeklyTask method every week (TimeSpan.FromDays(7)).
        // This method starts immediately (TimeSpan.Zero).
        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Set the timer to run once a week
            _timer = new Timer(DoWeeklyTask, null, TimeSpan.Zero, TimeSpan.FromDays(7));
            return Task.CompletedTask;
        }

        // This method is invoked by the timer every week
        private async void DoWeeklyTask(object state)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<StudyCenterDbContext>();

                // Fetch courses based on an appropriate property (assuming 'Schedule.EndDate' or a similar field instead of 'DueDate')
                var expiredCourses = await dbContext.Courses
                    .Where(c => c.Schedule.EndDate < DateTime.Now) 
                    .ToListAsync();

                foreach (var course in expiredCourses)
                {
                    // Remove associated lectures and lecture attendances
                    var lectures = dbContext.Lectures.Where(l => l.CourseId == course.CourseId).ToList(); // Changed 'Id' to 'LectureId'
                    var lectureIds = lectures.Select(l => l.LectureId).ToList();

                    var lectureAttendances = dbContext.LectureAttendances
                        .Where(la => lectureIds.Contains(la.LectureId));

                    dbContext.LectureAttendances.RemoveRange(lectureAttendances);
                    dbContext.Lectures.RemoveRange(lectures);

                    // Optionally remove the course if needed
                    dbContext.Courses.Remove(course);
                }

                await dbContext.SaveChangesAsync();
            }
        }

        // This method stops the timer by setting its interval to [Timeout.Infinite], which effectively disables it
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        // This method is used to clean up the timer resources when the service is disposed
        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
