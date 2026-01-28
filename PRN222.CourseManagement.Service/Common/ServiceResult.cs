namespace PRN222.CourseManagement.Service.Common
{
    public class ServiceResult
    {
        public bool IsSuccess { get; set; }

        // Thêm dấu ? (string?) để cho phép null
        public string? Message { get; set; }
        public object? Data { get; set; }

        public static ServiceResult Success(string msg = "Operation successful", object? data = null)
        {
            return new ServiceResult { IsSuccess = true, Message = msg, Data = data };
        }

        public static ServiceResult Fail(string msg)
        {
            return new ServiceResult { IsSuccess = false, Message = msg };
        }
    }
}