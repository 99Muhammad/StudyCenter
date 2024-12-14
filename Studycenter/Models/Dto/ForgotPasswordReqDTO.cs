using System.ComponentModel.DataAnnotations;

namespace SCMS_back_end.Models.Dto
{
    public class ForgotPasswordReqDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
