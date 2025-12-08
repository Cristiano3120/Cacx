using CacxShared.APIResponse;
using Cristiano3120.Logging;

namespace CacxClient.Abstractions;

internal interface IHttp
{
    public async Task<ApiResponse<T>> GetAsync<T>(CallerInfos callerInfos, string endpoint)
    {
       return default!;
    }

    public async Task<ApiResponse<bool>> DeleteAsync(CallerInfos callerInfos, string endpoint)
    {
        return default!;
    }

    public async Task<ApiResponse<TOutput>> PostAsync<TInput, TOutput>(TInput data, string endpoint, CallerInfos callerInfos)
    {
        return default!;
    }

    public async Task<ApiResponse<TOutput>> PutAsync<TInput, TOutput>(TInput data, string endpoint, CallerInfos callerInfos)
    {
        return default!;
    }
}
