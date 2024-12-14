namespace SCMS_back_end.Models.Dto.Grades
{
    public class StudentGradesResponseDTO
    {
        //public int CourseMarks { get; set; }
        public float OverallGrade { get; set; }
        public float QuizzesGrade { get; set; }
        public float AssignmentsGrade { get; set; }
        public List<AssignmentResponseDTO> Assignments { get; set; } = new List<AssignmentResponseDTO>();
        public List<QuizResponseDTO> Quizzes { get; set; } = new List<QuizResponseDTO>();
    }
}
