using SCMS_back_end.Models;
using SCMS_back_end.Models.Dto.Request;

namespace SCMS_back_end.Repositories.Interfaces
{
    public interface ICertificate
    {
        public Task CompleteGrading(int courseId);

    }
}
