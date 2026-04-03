using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace Dev4All.Web.Services;

public sealed class ApiClient(HttpClient httpClient) : IApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public async Task<T?> GetAsync<T>(string endpoint, CancellationToken ct = default)
    {
        using var response = await httpClient.GetAsync(endpoint, ct);
        await EnsureSuccessOrThrowAsync(response, ct);
        return await ReadResponseAsync<T>(response, ct);
    }

    public async Task<T?> PostAsync<T>(string endpoint, object body, CancellationToken ct = default)
    {
        using var content = CreateJsonContent(body);
        using var response = await httpClient.PostAsync(endpoint, content, ct);
        await EnsureSuccessOrThrowAsync(response, ct);
        return await ReadResponseAsync<T>(response, ct);
    }

    public async Task<T?> PutAsync<T>(string endpoint, object body, CancellationToken ct = default)
    {
        using var content = CreateJsonContent(body);
        using var response = await httpClient.PutAsync(endpoint, content, ct);
        await EnsureSuccessOrThrowAsync(response, ct);
        return await ReadResponseAsync<T>(response, ct);
    }

    public async Task DeleteAsync(string endpoint, CancellationToken ct = default)
    {
        using var response = await httpClient.DeleteAsync(endpoint, ct);
        await EnsureSuccessOrThrowAsync(response, ct);
    }

    private static StringContent CreateJsonContent(object body)
    {
        var json = JsonSerializer.Serialize(body, JsonOptions);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    private static async Task<T?> ReadResponseAsync<T>(HttpResponseMessage response, CancellationToken ct)
    {
        if (response.Content.Headers.ContentLength is 0)
        {
            return default;
        }

        var responseText = await response.Content.ReadAsStringAsync(ct);
        if (string.IsNullOrWhiteSpace(responseText))
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(responseText, JsonOptions);
    }

    private static async Task EnsureSuccessOrThrowAsync(HttpResponseMessage response, CancellationToken ct)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var responseText = response.Content is null
            ? string.Empty
            : await response.Content.ReadAsStringAsync(ct);

        throw response.StatusCode switch
        {
            HttpStatusCode.BadRequest => new BadHttpRequestException(
                string.IsNullOrWhiteSpace(responseText) ? "Validation error." : responseText),
            HttpStatusCode.Unauthorized => new UnauthorizedAccessException("Unauthorized. Please log in again."),
            HttpStatusCode.Forbidden => new InvalidOperationException("Access denied."),
            HttpStatusCode.NotFound => new KeyNotFoundException("Requested resource was not found."),
            HttpStatusCode.InternalServerError => new InvalidOperationException("An unexpected server error occurred."),
            _ => new HttpRequestException(
                $"Request failed with status code {(int)response.StatusCode}: {response.ReasonPhrase}")
        };
    }
}
