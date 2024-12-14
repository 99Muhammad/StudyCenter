using System.ComponentModel.DataAnnotations;

namespace SCMS_back_end.Models.Dto.Request
{
    public class QuestionUpdateDto
    {
        [Required]
        [MaxLength(1000)]
        public string Text { get; set; } = string.Empty;

        [Required]
        public int QuizId { get; set; }
    }


}
