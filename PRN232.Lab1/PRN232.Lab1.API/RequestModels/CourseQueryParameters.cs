namespace PRN232.Lab1.API.RequestModels
{
    public class CourseQueryParameters
    {
        /// <summary>
        /// Search by Course Name (e.g. 'PRN232')
        /// </summary>
        public string? Search { get; set; }

        /// <summary>
        /// Sort by courseName ascending ('courseName') or descending ('-courseName')
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
        /// Select specific fields to return, separated by commas (e.g. 'courseId,courseName')
        /// </summary>
        public string? Fields { get; set; } 

        /// <summary>
        /// Expand related entities (e.g. 'students' to load student information or 'semester' to load semester details)
        /// </summary>
        public string? Expand { get; set; } 
    }
}
