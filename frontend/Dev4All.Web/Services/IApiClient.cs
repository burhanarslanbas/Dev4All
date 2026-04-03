namespace Dev4All.Web.Services;

public interface IApiClient
{
    Task<T?> GetAsync<T>(string endpoint, CancellationToken ct = default);
    Task<T?> PostAsync<T>(string endpoint, object body, CancellationToken ct = default);
    Task<T?> PutAsync<T>(string endpoint, object body, CancellationToken ct = default);
    Task DeleteAsync(string endpoint, CancellationToken ct = default);
}
