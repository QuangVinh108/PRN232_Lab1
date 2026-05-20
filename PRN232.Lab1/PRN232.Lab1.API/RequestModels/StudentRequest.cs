using System;

namespace PRN232.Lab1.API.RequestModels
{
    public class StudentRequest
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
