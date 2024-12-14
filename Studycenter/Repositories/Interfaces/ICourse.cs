using SCMS_back_end.Models;
using SCMS_back_end.Models.Dto.Grades;
using SCMS_back_end.Models.Dto.Request;
using SCMS_back_end.Models.Dto.Response;
using System.Security.Claims;

namespace SCMS_back_end.Repositories.Interfaces
{
    public interface ICourse
    {
        //Create Course withouth teacher
        public Task<Course> CreateCourseWithoutTeacher(DtoCreateCourseWTRequest course);
        
        //Update Course Information
        public Task<Course> UpdateCourseInformation(int courseId, DtoUpdateCourseRequest course);

        // Get Course by Id
        public Task<DtoCourseResponse> GetCourseById(int courseId);

        // Get All Courses
        public Task<List<DtoCourseResponse>> GetAllCourses();
        
        // Get Courses that hasn't started yet
        public Task<List<DtoCourseResponse>> GetCoursesNotStarted(ClaimsPrincipal userPrincipal);

        // Delete Course
        public Task DeleteCourse(int courseId);

        // Get previous courses of a student
        public Task<List<DtoPreviousCourseResponse>> GetPreviousCoursesOfStudent(ClaimsPrincipal userPrincipal);

        // Get courses of a student
        public Task<List<DtoCourseResponse>> GetCoursesOfStudent(ClaimsPrincipal userPrincipal);

        // Get current courses of a student
        public Task<List<DtoCourseResponse>> GetCurrentCoursesOfStudent(ClaimsPrincipal userPrincipal);

        // Get courses of a teacher
        public Task<List<DtoCourseResponse>> GetCoursesOfTeacher(ClaimsPrincipal userPrincipal);

        // Get current courses of a teacher
        public Task<List<DtoCourseResponse>> GetCurrentCoursesOfTeacher(ClaimsPrincipal userPrincipal);

        // Calculate the average of a grades of a course
        public Task CalculateAverageGrade(int courseId);

        public Task<List<CourseGradesResponseDTO>> GetStudentCourseGrades(int courseId);
        public Task<StudentGradesResponseDTO> GetStudentGradesInCourse(int courseId, ClaimsPrincipal userPrincipal);

    }
}
