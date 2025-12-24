using CacxShared.APIResponse;
using Cristiano3120.Logging;

namespace CacxClient.Abstractions;

public interface IHttp
{
    Task<ApiResponse<T>> GetAsync<T>(CallerInfos callerInfos, string endpoint);
    Task<ApiResponse<bool>> DeleteAsync(CallerInfos callerInfos, string endpoint);
    Task<ApiResponse<TOutput>> PostAsync<TInput, TOutput>(TInput data, string endpoint, CallerInfos callerInfos);
    Task<ApiResponse<TOutput>> PutAsync<TInput, TOutput>(TInput data, string endpoint, CallerInfos callerInfos);
}
