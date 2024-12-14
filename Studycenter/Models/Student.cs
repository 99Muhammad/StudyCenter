using System.ComponentModel.DataAnnotations;

namespace SCMS_back_end.Models
{
    public class Student
    {
        [Key]
        public int StudentId { get; set; }
        public string UserId { get; set; } = string.Empty;

        //public int Level { get; set; }

        [Required]
        [MaxLength(255)]
        public string FullName { get; set; } = string.Empty;

        [MaxLength(255)]
        public string PhoneNumber { get; set; } = string.Empty;

        public User User { get; set; }  // Navigation property
        public ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();
        public ICollection<StudentAssignment> StudentAssignments { get; set; } = new List<StudentAssignment>();
        public ICollection<LectureAttendance> LectureAttendances { get; set; } = new List<LectureAttendance>();
        public ICollection<StudentQuiz> StudentQuizzes { get; set; } = new List<StudentQuiz>();
        public ICollection<QuizResult> QuizzesResult { get; set; } = new List<QuizResult>();
        public ICollection<StudentAnswer> StudentAnswers { get; set; } = new List<StudentAnswer>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }

}
