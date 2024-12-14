using System.ComponentModel.DataAnnotations;

namespace SCMS_back_end.Models.Dto.Request
{
    public class ResetPasswordReqDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        public string Token { get; set; }
    }
}
