using System;
using System.ComponentModel.DataAnnotations;

namespace PRN232.Lab1.API.RequestModels
{
    public class SemesterRequest
    {
        [Required(ErrorMessage = "Semester name is required")]
        [StringLength(100, ErrorMessage = "Semester name cannot exceed 100 characters")]
        public string SemesterName { get; set; } = null!;

        [Required(ErrorMessage = "Start date is required")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        public DateTime EndDate { get; set; }
    }
}
