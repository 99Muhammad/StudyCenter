using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SCMS_back_end.Models;
using SCMS_back_end.Repositories.Interfaces;
using SCMS_back_end.Models.Dto.Request;
using Microsoft.AspNetCore.Authorization;
using System.Runtime.InteropServices.JavaScript;

namespace SCMS_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles ="Admin")]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartment _departmentService;
        public DepartmentController(IDepartment context)
        {
            _departmentService = context;
        }

        // GET: api/Department
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Department>>> GetAllDepartments()
        {
            var department = await _departmentService.GetAllDepartmentsAsync();
            return Ok(department);
        }

        // GET: api/Department/5
        [Authorize(Roles= "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Department>> GetDepartment(int id)
        {
            var department = await _departmentService.GetDepartmentByIdAsync(id);
            if (department == null) return NotFound();
            return Ok(department);
        }

        // PUT: api/Department/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<Department>> UpdateDepartment(int id, string DepartmentName)
        {
            if (DepartmentName == null)
            {
                return BadRequest("Department Name is required.");
            }
            var updatedDepartment = await _departmentService.UpdateDepartmentAsync(id, DepartmentName);
            if (updatedDepartment == null) return NotFound();
            return Ok(updatedDepartment);
        }

        // POST: api/Department
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<User>> AddDepartment(string DepartmentName)
        {
            if (DepartmentName == null)
            {
                return BadRequest("Department Name is required.");
            }
            var department = await _departmentService.AddDepartmentAsync(DepartmentName);
            return CreatedAtAction(nameof(GetDepartment), new { id = department.DepartmentId }, department);
        }

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteDepartment(int id)
        //{
        //    var department = await _departmentService.GetDepartmentByIdAsync(id);
        //    if (department == null) return NotFound();

        //    var result = await _departmentService.DeleteDepartmentAsync(id);
        //    if (!result) return Conflict(new { Message = "Department has subjects with active courses" });
        //    return NoContent();
        //}


    }
}
