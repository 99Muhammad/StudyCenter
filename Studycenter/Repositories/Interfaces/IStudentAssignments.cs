using SCMS_back_end.Models;
using System.Threading.Tasks;
using SCMS_back_end.Models.Dto.Request;
using SCMS_back_end.Models.Dto.Response;
using SCMS_back_end.Models.Dto.Request.Assignment;
using System.Security.Claims;


namespace SCMS_back_end.Services
{
    public interface IStudentAssignments
    {
        //Task<StudentAssignment> AddOrUpdateStudentAssignmentAsync(StudentAssignmentDtoRequest studentAssignment);
        //AddStudentAssignmentSubmissionAsync StudentAssignmentSubmissionDtoRequest
        //    AddStudentAssignmentFeedbackAsync TeacherAssignmentFeedbackDtoRequest
        Task<StudentAssignment> AddStudentAssignmentSubmissionAsync(StudentAssignmentSubmissionDtoRequest studentAssignment, ClaimsPrincipal userPrincipal);
        Task<StudentAssignment> AddStudentAssignmentFeedbackAsync(TeacherAssignmentFeedbackDtoRequest studentAssignment);
        Task<StudentAssignment> AddGradeForNonSubmittedAssignmentAsync(DtoGradeNotSubmitted notSubmitted);
        Task<StudentAssignment> UpdateStudentAssignmentAsync(int studentAssignmentId, int? grade, string feedback);
        Task<StudentAssignmentDtoResponse> GetStudentAssignmentByIdAsync(int studentAssignmentId);
        Task<StudentAssignmentDtoResponse> GetStudentAssignmentByAssignmentAndStudentAsync(int assignmentId, int studentId);
    }

}
