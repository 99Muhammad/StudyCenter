﻿namespace SCMS_back_end.Models.Dto.Grades
{
    public class QuizResponseDTO
    {
        public string Title { get; set; } = string.Empty;
        public int FullMark { get; set; }
        public int AchievedMark { get; set; }
    }
}