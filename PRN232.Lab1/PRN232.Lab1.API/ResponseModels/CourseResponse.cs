using System;
using System.Collections.Generic;

namespace PRN232.Lab1.API.ResponseModels
{
    public class CourseResponse
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; } = null!;
        public int SemesterId { get; set; }
        public List<EnrollmentResponse>? Enrollments { get; set; }
        public List<StudentResponse>? Students { get; set; }
        public SemesterResponse? Semester { get; set; }
    }
}
