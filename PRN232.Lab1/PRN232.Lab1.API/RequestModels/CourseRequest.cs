using System.ComponentModel.DataAnnotations;

namespace PRN232.Lab1.API.RequestModels
{
    public class CourseRequest
    {
        [Required(ErrorMessage = "Course name is required")]
        [StringLength(100, ErrorMessage = "Course name cannot exceed 100 characters")]
        public string CourseName { get; set; } = null!;

        [Required(ErrorMessage = "Semester ID is required")]
        public int SemesterId { get; set; }
    }
}
