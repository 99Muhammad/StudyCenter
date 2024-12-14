using SCMS_back_end.Models.Dto.Request;
using System;
using System.Threading.Tasks;

namespace SCMS_back_end.Repositories.Interfaces
{
    public interface ILecture
    {
        Task AddLecturesAsync(int courseId); 
        //Task DeleteCourseAttendanceRecordsAsync(int courseId);
    }
}
