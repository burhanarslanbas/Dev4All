using System.Net;
using System.Text;
using System.Text.Json;

namespace Dev4All.Web.Services;

public sealed class ApiClient(HttpClient httpClient) : IApiClient
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task<T?> GetAsync<T>(string endpoint, CancellationToken ct = default)
    {
        using var response = await httpClient.GetAsync(endpoint, ct);
        await EnsureSuccessStatusCodeAsync(response, ct);
        return await DeserializeAsync<T>(response, ct);
    }

    public async Task<T?> PostAsync<T>(string endpoint, object body, CancellationToken ct = default)
    {
        using var response = await httpClient.PostAsync(endpoint, CreateJsonContent(body), ct);
        await EnsureSuccessStatusCodeAsync(response, ct);
        return await DeserializeAsync<T>(response, ct);
    }

    public async Task<T?> PutAsync<T>(string endpoint, object body, CancellationToken ct = default)
    {
        using var response = await httpClient.PutAsync(endpoint, CreateJsonContent(body), ct);
        await EnsureSuccessStatusCodeAsync(response, ct);
        return await DeserializeAsync<T>(response, ct);
    }

    public async Task DeleteAsync(string endpoint, CancellationToken ct = default)
    {
        using var response = await httpClient.DeleteAsync(endpoint, ct);
        await EnsureSuccessStatusCodeAsync(response, ct);
    }

    private static StringContent CreateJsonContent(object body)
    {
        var json = JsonSerializer.Serialize(body, _jsonOptions);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    private static async Task<T?> DeserializeAsync<T>(HttpResponseMessage response, CancellationToken ct)
    {
        if (response.Content.Headers.ContentLength is 0)
        {
            return default;
        }

        var content = await response.Content.ReadAsStringAsync(ct);
        if (string.IsNullOrWhiteSpace(content))
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(content, _jsonOptions);
    }

    private static async Task EnsureSuccessStatusCodeAsync(HttpResponseMessage response, CancellationToken ct)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var badRequestContent = await response.Content.ReadAsStringAsync(ct);
            throw new HttpRequestException(
                string.IsNullOrWhiteSpace(badRequestContent)
                    ? "Doğrulama hatası. Lütfen girdilerinizi kontrol edip tekrar deneyin."
                    : badRequestContent,
                null,
                response.StatusCode);
        }

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            throw new HttpRequestException(
                "Yetkisiz istek. Lütfen tekrar giriş yapın.",
                null,
                response.StatusCode);
        }

        if (response.StatusCode == HttpStatusCode.Forbidden)
        {
            throw new HttpRequestException(
                "Erişim reddedildi.",
                null,
                response.StatusCode);
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            throw new HttpRequestException(
                "İstenen kaynak bulunamadı.",
                null,
                response.StatusCode);
        }

        if (response.StatusCode == HttpStatusCode.InternalServerError)
        {
            throw new HttpRequestException(
                "Sunucuda beklenmeyen bir hata oluştu.",
                null,
                response.StatusCode);
        }

        var content = await response.Content.ReadAsStringAsync(ct);
        throw new HttpRequestException(
            string.IsNullOrWhiteSpace(content)
                ? $"API request failed with status code {(int)response.StatusCode}."
                : content,
            null,
            response.StatusCode);
    }
}
