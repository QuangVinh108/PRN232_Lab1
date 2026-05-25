using System;
using System.Collections.Generic;

namespace PRN232.Lab1.Services.BusinessModels
{
    public class SemesterBusinessModel
    {
        public int SemesterId { get; set; }
        public string SemesterName { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<CourseBusinessModel>? Courses { get; set; }
    }
}
