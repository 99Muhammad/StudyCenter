using System.ComponentModel.DataAnnotations;

namespace SCMS_back_end.Models
{
    public class WeekDay
    {
        [Key]
        public int WeekDayId { get; set; }

        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        public ICollection<ScheduleDay> ScheduleDays { get; set; } = new List<ScheduleDay>();
    }

}
