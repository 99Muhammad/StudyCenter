namespace SCMS_back_end.Models
{
    public class Classroom
    {
        public int ClassroomId { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
