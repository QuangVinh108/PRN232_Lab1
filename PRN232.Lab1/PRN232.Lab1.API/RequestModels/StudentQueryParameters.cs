namespace PRN232.Lab1.API.RequestModels
{
    public class StudentQueryParameters
    {
        /// <summary>
        /// Search by Student Full Name or Email (e.g. 'Nguyen')
        /// </summary>
        public string? Search { get; set; }

        /// <summary>
        /// Sort by fullName/dateOfBirth ascending ('fullName', 'dateOfBirth') or descending ('-fullName', '-dateOfBirth')
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
        /// Select specific fields to return, separated by commas (e.g. 'studentId,fullName,email')
        /// </summary>
        public string? Fields { get; set; } 

        /// <summary>
        /// Expand related entities (e.g. 'enrollments' to load enrolled courses information)
        /// </summary>
        public string? Expand { get; set; } 
    }
}
