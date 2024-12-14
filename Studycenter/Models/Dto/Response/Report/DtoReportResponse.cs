namespace SCMS_back_end.Models.Dto.Response.Report
{
    public class StudentEnrollmentOverview
    {
        public int TotalStudents { get; set; }
        public int ActiveCourses { get; set; }
        public double AverageEnrollmentPerCourse { get; set; }
    }

    public class CoursePerformanceOverview
    {
        public int PassingStudents { get; set; }
        public int FailingStudents { get; set; }
        public int DroppingStudents { get; set; }
        public double AverageGrade { get; set; }
        public List<CoursePerformance> TopPerformingCourses { get; set; }
        public List<CoursePerformance> UnderperformingCourses { get; set; }
    }

    public class CoursePerformance
    {
        public string CourseName { get; set; }
        public double AverageGrade { get; set; }
    }

    public class InstructorEffectivenessOverview
    {
        public List<InstructorCourseCount> InstructorCourses { get; set; }
        public List<InstructorPerformance> AverageStudentPerformance { get; set; }
        public List<InstructorPerformance> TopInstructors { get; set; }
    }

    public class InstructorCourseCount
    {
        public string InstructorName { get; set; }
        public int CourseCount { get; set; }
    }

    public class InstructorPerformance
    {
        public string InstructorName { get; set; }
        public double AveragePerformance { get; set; }
    }

    public class DepartmentalActivityOverview
    {
        public List<DepartmentActivity> DepartmentActivities { get; set; }
    }

    public class DepartmentActivity
    {
        public string DepartmentName { get; set; }
        public int TeacherCount { get; set; }
        public int SubjectCount { get; set; }
        public int StudentCount { get; set; }
    }

    public class SystemHealthCheckOverview
    {
        public int UnderCapacityCourses { get; set; }
        public int FullCapacityCourses { get; set; }
        public double AverageCourseUtilization { get; set; }
    }
}
