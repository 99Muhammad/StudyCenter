namespace SCMS_back_end.Models
{
    public class CourseAnnouncement
    {
        public int CourseAnnouncementId { get; set; }
        public int AnnouncementId { get; set; }
        public int CourseId { get; set; }
        public Announcement Announcement { get; set; }
        public Course Course { get; set; }
    }
}
