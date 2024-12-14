using SCMS_back_end.Models.Dto.Request.Assignment;
using SCMS_back_end.Models.Dto.Request.Teacher;
using SCMS_back_end.Models.Dto.Response.Teacher;

namespace SCMS_back_end.Repositories.Interfaces
{
    public interface ITeacher
    {
        Task<DtoAddTeacherResponse> AddTeacher(DtoAddTeacherRequest Teacher);

        Task<DtoUpdateTeacherInfoByAdminRequest> UpdateTeacherInfoByID(int TeacherID, DtoUpdateTeacherInfoByAdminRequest Teacher);

        Task<DtoGetTeacherInfoByIDRequest> GetTeacherInfoByID(int TeacherID);

        Task<List<DtoGetAllTeacherRequest>>GetAllTeachers();
        //Task DeleteTeacher(int TeacherID);
    }
}
