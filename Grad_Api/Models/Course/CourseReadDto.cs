﻿using Grad_Api.Models.Lessons;
namespace Grad_Api.Models.Course
{
    public class CourseReadDto : BaseDto
    {
        public string? Title { get; set; }

        public string? Description { get; set; }

        public string? TeacherName { get; set; }
        public string? CategoryName { get; set; }
        public int LessonCount { get; set; }
        public List<ReadLessonDto>? Lessons { get; set; }
    }
}
