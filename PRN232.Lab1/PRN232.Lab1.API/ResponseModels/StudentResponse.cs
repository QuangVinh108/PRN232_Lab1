namespace PRN232.Lab1.API.ResponseModels
{
    public class StudentResponse
    {
        public int StudentId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        // Có thể format lại ngày tháng ở đây nếu cần
        public DateTime DateOfBirth { get; set; }
        public List<EnrollmentResponse>? Enrollments { get; set; }
    }
}
