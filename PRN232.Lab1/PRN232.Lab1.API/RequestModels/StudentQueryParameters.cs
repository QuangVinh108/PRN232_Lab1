namespace PRN232.Lab1.API.RequestModels
{
    public class StudentQueryParameters
    {
        public string? Search { get; set; }
        public string? Sort { get; set; }
        public int Page { get; set; } = 1;      
        public int Size { get; set; } = 10;
        public string? Fields { get; set; } 
        public string? Expand { get; set; } 
    }
}
