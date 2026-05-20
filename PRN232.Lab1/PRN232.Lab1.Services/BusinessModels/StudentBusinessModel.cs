using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.Lab1.Services.BusinessModels
{
    public class StudentBusinessModel
    {
        public int StudentId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public List<EnrollmentBusinessModel>? Enrollments { get; set; }
    }
}
