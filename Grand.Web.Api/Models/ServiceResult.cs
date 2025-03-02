namespace Grand.Web.Api.Models;

public class ServiceResult<T>
{
    public bool IsSuccess { get; private set; }
    public string Message { get; private set; }
    public T Data { get; private set; }

    private ServiceResult(bool success, string message = null, T data = default)
    {
        IsSuccess = success;
        Message = message;
        Data = data;
    }

    public static ServiceResult<T> Success(T data, string message = null)
    {
        return new ServiceResult<T>(true, message, data);
    }

    public static ServiceResult<T> Error(string message)
    {
        return new ServiceResult<T>(false, message);
    }
} 