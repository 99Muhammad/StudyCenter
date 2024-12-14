namespace SCMS_back_end.Models.Dto.Response.Announcement
{
    public class DtoGetAnnouncementRes
    {
        public int AnnouncementId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string Type { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
    }
}
