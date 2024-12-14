namespace SCMS_back_end.Models
{
    public class Audience
    {
        public int AudienceId { get; set; }
        public string Name { get; set; }
        public ICollection<Announcement> Annoumcements { get; set; } = new List<Announcement>();

    }
}
