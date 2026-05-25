namespace PRN232.Lab1.API.ResponseModels
{
    public class EnrollmentResponse
    {
        public int EnrollmentId { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public DateTime EnrollDate { get; set; }
        public string Status { get; set; } = null!;
        public StudentResponse? Student { get; set; }
        public CourseResponse? Course { get; set; }
    }
}
