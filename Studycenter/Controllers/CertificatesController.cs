using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SCMS_back_end.Repositories.Interfaces;

namespace SCMS_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CertificatesController : ControllerBase
    {
        private readonly ICertificate _certificate;
        private readonly IWebHostEnvironment _environment;

        public CertificatesController(ICertificate certificate, IWebHostEnvironment environment)
        {
            _certificate = certificate;
            _environment = environment;
        }

        [Authorize(Roles = "Teacher")]
        [HttpGet("{courseId}/complete-grading")]
        public async Task<ActionResult> CompleteGrading(int courseId)
        {
            try
            {
                await _certificate.CompleteGrading(courseId);
                return Ok("Test successful");

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Roles = "Student")]
        [HttpGet("download/{certificateId}")]
        public IActionResult DownloadCertificate(string certificateId)
        {
            // Build the full path to the certificate
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Certificates");
            string filePath = Path.Combine(folderPath, certificateId+".pdf");

            // Check if the file exists
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound(new { Message = "Certificate not found" });
            }

            // Serve the file as a download
            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/pdf", certificateId);
        }
    }
}
