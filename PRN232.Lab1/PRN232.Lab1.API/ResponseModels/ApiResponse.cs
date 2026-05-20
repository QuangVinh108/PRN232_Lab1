namespace PRN232.Lab1.API.ResponseModels
{
    public class PaginationMetadata
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
    }
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public PaginationMetadata? Pagination { get; set; }
        public object Errors { get; set; }

        // Bạn có thể tạo thêm các constructor tiện ích ở đây
        public static ApiResponse<T> Ok(T data, PaginationMetadata? pagination = null, string message = "Request processed successfully")
        {
            return new ApiResponse<T> { Success = true, Message = message, Data = data, Pagination = pagination };
        }

        public static ApiResponse<T> NotFound(string message = "Resource not found")
        {
            return new ApiResponse<T> { Success = false, Message = message, Errors = "404" };
        }
    }
}
