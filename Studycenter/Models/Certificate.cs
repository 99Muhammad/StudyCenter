namespace SCMS_back_end.Models
{
    public class Certificate
    {
        public string CertificateId { get; set; }= string.Empty;
        public int StudentCourseId { get; set; }
        public DateTime CompletionDate { get; set; }
        public string CertificatePath { get; set; } = string.Empty;
        public StudentCourse? StudentCourse { get; set; }
    }
}
