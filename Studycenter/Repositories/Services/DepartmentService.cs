using Microsoft.EntityFrameworkCore;
using SCMS_back_end.Data;
using SCMS_back_end.Models;
using SCMS_back_end.Models.Dto.Request;
using SCMS_back_end.Repositories.Interfaces;

namespace SCMS_back_end.Repositories.Services
{
    public class DepartmentService : IDepartment
    {
        StudyCenterDbContext _context;
        public DepartmentService(StudyCenterDbContext context)
        {
            _context = context;
        }
        public async Task<DtoDepartment> AddDepartmentAsync(string DepartmentName)
        {
            var department = new Department { Name = DepartmentName };
            await _context.Departments.AddAsync(department).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return new DtoDepartment
            {
                DepartmentId = department.DepartmentId,
                DepartmentName = department.Name
            };
        }

        /* //Delete department
         * //private async Task<bool> _HasCurrentCoursesInDepartment(int departmentId)
        //{
        //    var result = await _context.Departments.Where(d => d.DepartmentId == departmentId)
        //            .SelectMany(d => d.Subjects).SelectMany(s => s.Courses)
        //            .Where(c => c.Schedule.EndDate > DateTime.Now).ToListAsync();
        //    return result.Any();
        //}
        //public async Task<bool> DeleteDepartmentAsync(int id)
        //{
        //    var department = await _context.Departments.FindAsync(id).ConfigureAwait(false);

        //    if (department != null && !await _HasCurrentCoursesInDepartment(department.DepartmentId))
        //    {
        //        _context.Departments.Remove(department);
        //        await _context.SaveChangesAsync().ConfigureAwait(false);
        //        return true;
        //    }
        //    return false;
        //}
        */
        public async Task<IEnumerable<DtoDepartment>> GetAllDepartmentsAsync()
        {
            var departments = await _context.Departments.ToListAsync().ConfigureAwait(false);
            var departmentsDto = departments.Select(d => new DtoDepartment
            {
                DepartmentId = d.DepartmentId,
                DepartmentName = d.Name
            }).ToList();
            return departmentsDto;
        }

        public async Task<DtoDepartment> GetDepartmentByIdAsync(int id)
        {
            var department = await _context.Departments.FindAsync(id).ConfigureAwait(false);
            if (department != null)
            {
                return new DtoDepartment
                {
                    DepartmentId = department.DepartmentId,
                    DepartmentName = department.Name
                };
            }
            return null;
        }

        public async Task<DtoDepartment> UpdateDepartmentAsync(int id, string DepartmentName)
        {
            var department = await _context.Departments.FindAsync(id).ConfigureAwait(false);
            if (department != null)
            {
                department.Name = DepartmentName;
                _context.Departments.Update(department);
                await _context.SaveChangesAsync().ConfigureAwait(false);
                return new DtoDepartment
                {
                    DepartmentId = department.DepartmentId,
                    DepartmentName = department.Name
                };
            }
            return null;
        }
    }
}
