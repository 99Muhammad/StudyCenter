using Microsoft.AspNetCore.Identity;

namespace SCMS_back_end.Models
{
    public class Announcement
    {
        public int AnnouncementId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
        public int AudienceId { get; set; }
        public User User { get; set; }
        public Audience Audience { get; set; }
        public CourseAnnouncement CourseAnnouncement { get; set; }
    }
}
