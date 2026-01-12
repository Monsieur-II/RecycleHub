namespace RecycleHub.Utils;

public sealed record ApiResponse<T>(string Message, int Code, T? Data)
{
    public static ApiResponse<T> Fail(string internalServerError="Internal Server Error", int status500InternalServerError=500)
    {
        return new ApiResponse<T>(internalServerError, status500InternalServerError, default);
    }
}
