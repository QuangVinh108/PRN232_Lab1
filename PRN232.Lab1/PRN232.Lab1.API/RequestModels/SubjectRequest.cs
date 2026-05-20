using System.ComponentModel.DataAnnotations;

namespace PRN232.Lab1.API.RequestModels
{
    public class SubjectRequest
    {
        [Required(ErrorMessage = "Subject code is required")]
        [StringLength(20, ErrorMessage = "Subject code cannot exceed 20 characters")]
        public string SubjectCode { get; set; } = null!;

        [Required(ErrorMessage = "Subject name is required")]
        [StringLength(100, ErrorMessage = "Subject name cannot exceed 100 characters")]
        public string SubjectName { get; set; } = null!;

        [Required(ErrorMessage = "Credit is required")]
        [Range(1, 10, ErrorMessage = "Credit must be between 1 and 10")]
        public int Credit { get; set; }
    }
}
