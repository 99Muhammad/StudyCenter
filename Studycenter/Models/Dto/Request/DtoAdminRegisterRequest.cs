using System.ComponentModel.DataAnnotations;

namespace SCMS_back_end.Models.Dto.Request
{
    public class DtoAdminRegisterRequest
    {
        public string Username { get; set; }

        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }

    }
}
