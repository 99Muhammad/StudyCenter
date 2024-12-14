using Azure.Core;
using Microsoft.EntityFrameworkCore;
using SCMS_back_end.Data;
using SCMS_back_end.Models;
using SCMS_back_end.Models.Dto.Request;
using SCMS_back_end.Repositories.Interfaces;
using SendGrid.Helpers.Mail.Model;
using System.IO;

namespace SCMS_back_end.Repositories.Services
{
    public class CertificateService:ICertificate
    {
        private readonly PdfService _pdfService;
        private readonly StudyCenterDbContext _context;

        public CertificateService(PdfService pdfService, StudyCenterDbContext context)
        {
            _pdfService = pdfService;
            _context = context;
        }

        private static readonly string ImagePath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "no-back-80.png");

        private string ConvertImageToBase64(string imagePath)
        {
            byte[] imageBytes = File.ReadAllBytes(imagePath);
            return Convert.ToBase64String(imageBytes);
        }

        private string _GenerateCertificate(string studentName, string courseName, string certificateId, DateTime completionDate)
        {
            string base64Image = ConvertImageToBase64(ImagePath);

            string htmlTemplate = $@"
        <html>
        <head>
      <style type=""text/css"">
      body,
      html {{
        margin: 0;
        padding: 0;
      }}
      body {{
        color: black;
        display: table;
        font-family: Georgia, serif;
        font-size: 24px;
        text-align: center;
      }}
      .container {{
        border: 20px solid #4a9b8f;
        width: 750px;
        height: 563px;
        display: table-cell;
        vertical-align: middle;
        padding: 20px;
      }}
      .logo {{
        display: inline-block;
        text-align: center;
        vertical-align: middle;
        color: #4a9b8f;
      }}
      .marquee {{
        color: #4a9b8f;
        font-size: 48px;
        margin: 20px;
      }}
      .assignment {{
        margin: 20px;
      }}
      .person {{
        border-bottom: 2px solid black;
        font-size: 32px;
        font-style: italic;
        margin: 20px auto;
        width: 400px;
        font-weight: 600;
      }}
      .reason {{
        margin: 20px;
      }}
      .course {{
        margin: 20px;
        font-weight: 600;
      }}
      .image {{
        height: 60px;
        width: 60px;
      }}
      .date {{
        margin: 20px;
        font-size: 20px;
      }}
      .id {{
        margin: 20px;
        margin-top: 50px;
        font-size: 20px;
        color: grey;
      }}
    </style>
        </head>
        <body>
            <div class=""container"">
               
                <div class=""marquee"">Certificate of Completion</div>
                <div class=""assignment"">This certificate is awarded to</div>
                <div class=""person"">{studentName}</div>
                <div class=""reason"">for successfully completing</div>
                <div class=""course"">{courseName}</div>
                <div class=""date"">Date: {completionDate.ToString("MMMM dd, yyyy")}</div>
                <div class=""id"">Certificate no: {certificateId}</div>
            </div>
        </body>
        </html>";
            var pdfBytes = _pdfService.GenerateCertificate(htmlTemplate);
            var filePath = Path.Combine("Certificates", $"{certificateId}.pdf");
            System.IO.File.WriteAllBytes(filePath, pdfBytes);

            return filePath;
        }

        private async Task PostCertificate(int studentCourseId)
        {
            var studentCourse= await _context.StudentCourses
                .Include(sc => sc.Student)
                .Include(sc => sc.Course)
                .FirstOrDefaultAsync(sc => sc.StudentCourseId == studentCourseId);
            var certificateId= Guid.NewGuid().ToString();
            Console.WriteLine("this is the certificate Id",certificateId);
            var newCertificate = new Certificate
            {
               CertificateId = certificateId,
               StudentCourseId = studentCourseId,
               CompletionDate = DateTime.Now,
               CertificatePath= _GenerateCertificate(studentCourse.Student.FullName, studentCourse.Course.ClassName, certificateId, DateTime.Now)
            };
            _context.Certificates.Add(newCertificate);
            await _context.SaveChangesAsync();
        }

        // complete grading and generate certificates
        public async Task CompleteGrading(int courseId)
        {
            var course = await _context.Courses.Include(c => c.StudentCourses).ThenInclude(sc => sc.Student).
                Include(c=>c.Schedule).
                Where(c => c.CourseId == courseId).FirstOrDefaultAsync();
            if (course == null)
            {
                throw new Exception("Course not found");
            }
            if (course.Schedule.EndDate > DateTime.Now)
            {
                throw new Exception("Grading cannot be completed before the course ends on " + course.Schedule.EndDate.ToString("MM/dd/yyyy"));
            }
            course.IsCompleted= true;
            await _context.SaveChangesAsync();
            foreach (var studentCourse in course.StudentCourses)
            {
                await PostCertificate(studentCourse.StudentCourseId);
            }
        }


    }


    //public async Task PostCertificate(DtoCertificateRequest dto)
    //{
    //    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Certificates");
    //    if (!Directory.Exists(uploadsFolder))
    //    {
    //        Directory.CreateDirectory(uploadsFolder);
    //    }


    //    var fileExtension = Path.GetExtension(dto.CertificateFile.FileName);
    //    var newFileName = $"{Guid.NewGuid()}{fileExtension}";
    //    var filePath = Path.Combine(uploadsFolder, newFileName);

    //    using (var stream = new FileStream(filePath, FileMode.Create))
    //    {
    //        await dto.CertificateFile.CopyToAsync(stream);
    //    }

    //    var newCertificate = new Certificate
    //    {
    //        StudentId = dto.StudentId,
    //        CourseId = dto.CourseId,
    //        CompletionDate = dto.CompletionDate,
    //        CertificatePath = filePath
    //    };

    //    _context.Certificates.Add(newCertificate);
    //    await _context.SaveChangesAsync();
    //}
}
