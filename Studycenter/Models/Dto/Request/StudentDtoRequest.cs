using System.ComponentModel.DataAnnotations;

namespace SCMS_back_end.Models.Dto.Request
{
    public class StudentDtoRequest
    {
        //public string UserId { get; set; }
        //public int Level { get; set; }

        [Required]
        [MaxLength(255)]
        public string FullName { get; set; } = string.Empty;

        [MaxLength(255)]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
