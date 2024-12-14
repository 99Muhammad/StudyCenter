using System.ComponentModel.DataAnnotations;

namespace SCMS_back_end.Models
{
    public class ScheduleDay
    {
        [Key]
        public int ScheduleDayId { get; set; }

        [Required]
        public int ScheduleId { get; set; }

        [Required]
        public int WeekDayId { get; set; }

        public Schedule Schedule { get; set; }  // Navigation property
        public WeekDay WeekDay { get; set; }  // Navigation property
    }

}
