namespace SCMS_back_end.Models.Dto.Grades
{
    public class CourseGradesResponseDTO
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public float averageGrades { get; set; }
        public float assignments { get; set; }
        public float quizzes { get; set; }
    }
}
