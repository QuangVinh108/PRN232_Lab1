namespace PRN232.Lab1.API.RequestModels
{
    public class SubjectQueryParameters
    {
        /// <summary>
        /// Search by Subject Code or Subject Name (e.g. 'PRN232')
        /// </summary>
        public string? Search { get; set; }

        /// <summary>
        /// Sort by subjectName/credit ascending ('subjectName', 'credit') or descending ('-subjectName', '-credit')
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
        /// Select specific fields to return, separated by commas (e.g. 'subjectId,subjectCode,subjectName,credit')
        /// </summary>
        public string? Fields { get; set; } 

        /// <summary>
        /// Expand related entities (No relations mapped for Subject yet)
        /// </summary>
        public string? Expand { get; set; } 
    }
}
