using System;
using System.ComponentModel.DataAnnotations;

namespace PRN232.Lab1.API.RequestModels
{
    public class EnrollmentRequest
    {
        [Required(ErrorMessage = "Student ID is required")]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "Course ID is required")]
        public int CourseId { get; set; }

        [Required(ErrorMessage = "Enroll date is required")]
        public DateTime EnrollDate { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [StringLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
        public string Status { get; set; } = null!;
    }
}
