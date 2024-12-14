﻿using System.ComponentModel.DataAnnotations;

namespace SCMS_back_end.Models.Dto.Request.Teacher
{
    public class DtoAddTeacherRequest
    {
        //[Key]
       // public int TeacherId { get; set; }
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string FullName { get; set; } = string.Empty;

        public int CourseLoad { get; set; }

        public string PhoneNumber { get; set; } = string.Empty;

       // public int DepartmentId { get; set; }


    }
}