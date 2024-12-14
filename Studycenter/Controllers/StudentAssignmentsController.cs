using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SCMS_back_end.Data;
using SCMS_back_end.Models;
using SCMS_back_end.Models.Dto.Request;
using SCMS_back_end.Models.Dto.Request.Assignment;
using SCMS_back_end.Models.Dto.Response;
using SCMS_back_end.Services;
using System;
using System.Threading.Tasks;

namespace SCMS_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentAssignmentsController : ControllerBase
    {
        private readonly IStudentAssignments _studentAssignmentsService;
        private readonly StudyCenterDbContext _context;

        public StudentAssignmentsController(IStudentAssignments studentAssignmentsService, StudyCenterDbContext context)
        {
            _studentAssignmentsService = studentAssignmentsService;
            _context = context; // Assign the injected context to the field
        }

        // GET: api/StudentAssignments
        [Authorize(Roles = "Teacher, Student")]
        [HttpGet("download/{studentAssignmentId}")]
        public async Task<IActionResult> DownloadFile(int studentAssignmentId)
        {
            // Get the student assignment from the service
            var studentAssignment = await _studentAssignmentsService.GetStudentAssignmentByIdAsync(studentAssignmentId);

            // Check if the student assignment or the Submission field is null
            if (studentAssignment == null || string.IsNullOrEmpty(studentAssignment.FilePath))
            {
                return NotFound("File not found or no submission.");
            }

            // Get the absolute file path
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", studentAssignment.FilePath);

            // Log the path for debugging
            Console.WriteLine($"Attempting to download file from path: {filePath}");

            // Check if the file exists on the server
            if (!System.IO.File.Exists(filePath))
            {
                // Log if the file doesn't exist
                Console.WriteLine($"File not found at path: {filePath}");
                return NotFound("File does not exist.");
            }

            try
            {
                // Create memory stream and copy file contents to it
                var memory = new MemoryStream();
                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    await stream.CopyToAsync(memory);
                }

                // Reset memory stream position for returning the file
                memory.Position = 0;

                // Infer the file's content type
                var contentType = "application/octet-stream";
                var fileName = Path.GetFileName(filePath);

                // Return the file for download
                return File(memory, contentType, fileName);
            }
            catch (Exception ex)
            {
                // Log exception details for debugging
                Console.WriteLine($"Error occurred while downloading the file: {ex.Message}");
                return StatusCode(500, "An error occurred while processing the file.");
            }
        }

        // POST: api/StudentAssignments
        [Authorize(Roles = "Student")]
        [HttpPost("submit-assignment")]
        public async Task<IActionResult> SubmitAssignment([FromForm] StudentAssignmentSubmissionDtoRequest dto)
        {
            try
            {
                var result = await _studentAssignmentsService.AddStudentAssignmentSubmissionAsync(dto, User);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");

            }
        }

        // POST: api/StudentAssignments/AddStudentAssignmentFeedback
        [Authorize(Roles = "Teacher")]
        [HttpPost("AddStudentAssignmentFeedback")]
        public async Task<IActionResult> AddStudentAssignmentFeedback([FromBody] TeacherAssignmentFeedbackDtoRequest dto)
        {
            try
            {
                var result = await _studentAssignmentsService.AddStudentAssignmentFeedbackAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");

            }
        }

        // POST: api/StudentAssignments/AddGradeForNonSubmittedAssignment
        [Authorize(Roles = "Teacher")]
        [HttpPost("AddGradeForNonSubmittedAssignment")]
        public async Task<IActionResult> AddGradeForNonSubmittedAssignment(DtoGradeNotSubmitted notSubmitted)
        {
            try
            {
                var result = await _studentAssignmentsService.AddGradeForNonSubmittedAssignmentAsync(notSubmitted);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");

            }
        }
        // PUT: api/StudentAssignments/update/{studentAssignmentId}
        [Authorize(Roles = "Teacher")]
        [HttpPut("update/{studentAssignmentId}")]
        public async Task<IActionResult> UpdateStudentAssignment(int studentAssignmentId, [FromBody] UpdateStudentAssignmentRequest request)
        {
            if (studentAssignmentId <= 0 || request == null)
            {
                return BadRequest("Invalid input data.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _studentAssignmentsService.UpdateStudentAssignmentAsync(studentAssignmentId, request.Grade, request.Feedback);
                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        // GET: api/StudentAssignments/5
        [Authorize(Roles = "Teacher, Student")]
        [HttpGet("{studentAssignmentId}")]
        public async Task<StudentAssignmentDtoResponse> GetStudentAssignmentByIdAsync(int studentAssignmentId)
        {
            var studentAssignment = await _context.StudentAssignments
                .Include(sa => sa.Assignment)
                .Include(sa => sa.Student)
                .FirstOrDefaultAsync(sa => sa.StudentAssignmentId == studentAssignmentId);

            if (studentAssignment == null)
            {
                return null;
            }

            var responseDto = new StudentAssignmentDtoResponse
            {
                StudentAssignmentId = studentAssignment.StudentAssignmentId,
                AssignmentId = studentAssignment.AssignmentId,
                StudentId = studentAssignment.StudentId,
                SubmissionDate = studentAssignment.SubmissionDate ?? DateTime.Now, 
                Submission = studentAssignment.Submission,
                Grade = studentAssignment.Grade,
                Feedback = studentAssignment.Feedback,
                FilePath = studentAssignment.FilePath
            };

            return responseDto;
        }
       
        // TODO
        //[Authorize(Roles = "Teacher,Student")]
        [HttpGet("byAssignmentAndStudent")]
        public async Task<StudentAssignmentDtoResponse> GetStudentAssignmentByAssignmentAndStudentAsync(int assignmentId, int studentId)
        {
            var studentAssignment = await _context.StudentAssignments
                .Include(sa => sa.Assignment)
                .Include(sa => sa.Student)
                .FirstOrDefaultAsync(sa => sa.AssignmentId == assignmentId && sa.StudentId == studentId);

            if (studentAssignment == null)
            {
                return null;
            }

            var responseDto = new StudentAssignmentDtoResponse
            {
                StudentAssignmentId = studentAssignment.StudentAssignmentId,
                AssignmentId = studentAssignment.AssignmentId,
                StudentId = studentAssignment.StudentId,
                SubmissionDate = studentAssignment.SubmissionDate ?? DateTime.Now, // Add fallback value if null
                Submission = studentAssignment.Submission,
                Grade = studentAssignment.Grade,
                Feedback = studentAssignment.Feedback,
                FilePath = studentAssignment.FilePath
            };

            return responseDto;
        }
    }
}
