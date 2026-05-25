namespace PRN232.Lab1.API.RequestModels
{
    public class EnrollmentQueryParameters
    {
        /// <summary>
        /// Search by Enrollment Status (e.g. 'Active')
        /// </summary>
        public string? Search { get; set; }

        /// <summary>
        /// Sort by enrollDate ascending ('enrollDate') or descending ('-enrollDate')
        /// </summary>
        public string? Sort { get; set; }

        /// <summary>
        /// Page number to retrieve, starting from 1
        /// </summary>
        public int Page { get; set; } = 1;      

        /// <summary>
        /// Maximum number of items per page
        /// </summary>
        public int Size { get; set; } = 10;

        /// <summary>
        /// Select specific fields to return, separated by commas (e.g. 'enrollmentId,status,enrollDate')
        /// </summary>
        public string? Fields { get; set; } 

        /// <summary>
        /// Expand related entities (e.g. 'student' to load student details, 'course' to load course details)
        /// </summary>
        public string? Expand { get; set; } 
    }
}
