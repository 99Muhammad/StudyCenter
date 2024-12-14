using System.ComponentModel.DataAnnotations.Schema;

namespace SCMS_back_end.Models.Dto.Response
{
    public class DtoCourseResponse
    {
        public int CourseId { get; set; }
        public string TeacherName { get; set; } = string.Empty;
        public int? TeacherId { get; set; }
        public string TeacherDepartment { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public List<string> Days { get; set; } = new List<string>();
        public string ClassName { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public string ClassroomNumber { get; set; } = string.Empty;
        public bool IsComplete { get; set; }
        public int Marks { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? NumberOfHours { get; set; }
        public string CertificateId { get; set; } = string.Empty;
        public float AverageGrades { get; set; }
        public  string Level { get; set; } = string.Empty;
        public int NumberOfEnrolledStudents { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
    }
}
