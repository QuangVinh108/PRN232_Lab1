using System;
using System.Collections.Generic;

namespace PRN232.Lab1.Services.BusinessModels
{
    public class CourseBusinessModel
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; } = null!;
        public int SemesterId { get; set; }
        public List<EnrollmentBusinessModel>? Enrollments { get; set; }
    }
}
