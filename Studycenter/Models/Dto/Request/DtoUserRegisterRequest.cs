using System.ComponentModel.DataAnnotations;

namespace SCMS_back_end.Models.Dto.Request
{
    public class DtoUserRegisterRequest
    {
        public string Username { get; set; }

        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        //public int Level { get; set; }
        public int DepartmentId { get; set; }
        public int CourseLoad { get; set; }
    }
}
