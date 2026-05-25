namespace PRN232.Lab1.API.RequestModels
{
    public class SemesterQueryParameters
    {
        /// <summary>
        /// Search by Semester Name (e.g. 'Summer 2024')
        /// </summary>
        public string? Search { get; set; }

        /// <summary>
        /// Sort by semesterName ascending ('semesterName') or descending ('-semesterName')
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
        /// Select specific fields to return, separated by commas (e.g. 'semesterId,semesterName,startDate,endDate')
        /// </summary>
        public string? Fields { get; set; } 

        /// <summary>
        /// Expand related entities (e.g. 'courses' to load courses in this semester)
        /// </summary>
        public string? Expand { get; set; } 
    }
}
