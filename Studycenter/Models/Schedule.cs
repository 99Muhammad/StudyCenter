using System.ComponentModel.DataAnnotations;

namespace SCMS_back_end.Models
{
    public class Schedule
    {
        [Key]
        public int ScheduleId { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public Course Course { get; set; } 
        public ICollection<ScheduleDay> ScheduleDays { get; set; } = new List<ScheduleDay>();
    }

}
