using SCMS_back_end.Models;
using SCMS_back_end.Models.Dto.Request;

namespace SCMS_back_end.Repositories.Interfaces
{
    public interface IDepartment
    {
        Task<IEnumerable<DtoDepartment>> GetAllDepartmentsAsync();
        Task<DtoDepartment> GetDepartmentByIdAsync(int id);
        Task<DtoDepartment> AddDepartmentAsync(string DepartmentName);
        Task<DtoDepartment> UpdateDepartmentAsync(int id, string DepartmentName);
        //Task<bool> DeleteDepartmentAsync(int id);
    }
}
