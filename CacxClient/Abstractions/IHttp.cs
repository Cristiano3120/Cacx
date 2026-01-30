using CacxClient.Services;
using Cristiano3120.Logging;

namespace CacxClient.Abstractions;

public interface IHttp
{
    Task<ApiResponse<T>> GetAsync<T>(string endpoint, CallerInfos callerInfos);
    Task<ApiResponse<bool>> DeleteAsync(string endpoint, CallerInfos callerInfos);
    Task<ApiResponse<TOutput>> PostAsync<TInput, TOutput>(TInput data, string endpoint, CallerInfos callerInfos);
    Task<ApiResponse<TOutput>> PutAsync<TInput, TOutput>(TInput data, string endpoint, CallerInfos callerInfos);
}