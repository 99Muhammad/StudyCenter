using SCMS_back_end.Models;
using SCMS_back_end.Models.Dto;
using SCMS_back_end.Models.Dto.Response;
using SCMS_back_end.Models.Dto.Request;

namespace SCMS_back_end.Repositories.Interfaces
{
    public interface ISubject
    {
        Task<IEnumerable<DtoSubjectResponse>> GetAllSubjectsAsync();
        Task<DtoSubjectResponse> GetSubjectByIdAsync(int id);
        Task<DtoSubjectResponse> AddSubjectAsync(DtoSubjectRequest subjectDto);
        Task<DtoSubjectResponse> UpdateSubjectAsync(int id, DtoSubjectRequest subjectDto);
        //Task<bool> DeleteSubjectAsync(int id);
        
    }
}
