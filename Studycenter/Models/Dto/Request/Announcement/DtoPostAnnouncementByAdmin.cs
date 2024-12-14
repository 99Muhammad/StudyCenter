namespace SCMS_back_end.Models.Dto.Request.Announcement
{
    public class DtoPostAnnouncementByAdmin
    {
       
        //public string UserId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        //public DateTime CreatedAt { get; set; }
        public string Audience { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;

    }
}
