using SCMS_back_end.Models.Dto.Request;
using SCMS_back_end.Models.Dto.Response;

namespace SCMS_back_end.Repositories.Interfaces
{
    public interface IClassroom
    {
        Task<IEnumerable<DtoClassroomResponse>> GetAllClassroomsAsync();
        Task<DtoClassroomResponse> GetClassroomByIdAsync(int id);
        Task<DtoClassroomResponse> AddClassroomAsync(DtoCreateClassroomRequest classroom);
        Task<DtoClassroomResponse> UpdateClassroomAsync(int id, DtoUpdateClassroomRequest classroom);
        Task<bool> DeleteClassroomAsync(int id);
    }
}
