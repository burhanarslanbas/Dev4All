using System.Collections.Concurrent;
using System.Net;
using System.Text.Json;

namespace Dev4All.Infrastructure.Email;

/// <summary>
/// Simple file-based HTML email renderer.
/// Loads <c>Email/Templates/&lt;key&gt;.html</c> and <c>layout.html</c> from the
/// application output directory and substitutes <c>{{Placeholder}}</c> tokens
/// with HTML-encoded values from the supplied payload dictionary.
/// </summary>
public sealed class TemplateRenderer
{
    private static readonly ConcurrentDictionary<string, string> TemplateCache = new();
    private readonly string _templatesRoot;

    public TemplateRenderer()
    {
        _templatesRoot = Path.Combine(AppContext.BaseDirectory, "Email", "Templates");
    }

    /// <summary>
    /// Renders the given template with the supplied payload JSON, wrapped in <c>layout.html</c>.
    /// </summary>
    /// <param name="templateKey">File name without extension, e.g. <c>verify-email</c>.</param>
    /// <param name="subject">Subject string, also exposed as <c>{{Subject}}</c> inside the layout.</param>
    /// <param name="payloadJson">JSON object whose top-level keys become placeholder names.</param>
    public string Render(string templateKey, string subject, string payloadJson)
    {
        var placeholders = ParsePayload(payloadJson);
        placeholders["Subject"] = subject;

        var bodyTemplate = LoadTemplate(templateKey);
        var renderedBody = Substitute(bodyTemplate, placeholders);

        var layoutTemplate = LoadTemplate("layout");

        var layoutPlaceholders = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["Subject"] = subject,
            ["Content"] = renderedBody
        };

        return SubstituteLayout(layoutTemplate, layoutPlaceholders);
    }

    private string LoadTemplate(string key)
    {
        return TemplateCache.GetOrAdd(key, k =>
        {
            var path = Path.Combine(_templatesRoot, $"{k}.html");
            if (!File.Exists(path))
                throw new FileNotFoundException($"Email template '{k}' not found at '{path}'.", path);

            return File.ReadAllText(path);
        });
    }

    private static Dictionary<string, string> ParsePayload(string payloadJson)
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (string.IsNullOrWhiteSpace(payloadJson))
            return result;

        using var document = JsonDocument.Parse(payloadJson);
        if (document.RootElement.ValueKind != JsonValueKind.Object)
            return result;

        foreach (var property in document.RootElement.EnumerateObject())
        {
            result[property.Name] = property.Value.ValueKind switch
            {
                JsonValueKind.String => property.Value.GetString() ?? string.Empty,
                JsonValueKind.Null => string.Empty,
                _ => property.Value.ToString()
            };
        }

        return result;
    }

    private static string Substitute(string template, IReadOnlyDictionary<string, string> values)
    {
        return SubstituteCore(template, values, htmlEncode: true);
    }

    private static string SubstituteLayout(string template, IReadOnlyDictionary<string, string> values)
    {
        // The Content placeholder already contains rendered HTML; avoid double-encoding it.
        return SubstituteCore(template, values, htmlEncode: false);
    }

    private static string SubstituteCore(string template, IReadOnlyDictionary<string, string> values, bool htmlEncode)
    {
        var result = template;
        foreach (var pair in values)
        {
            var token = "{{" + pair.Key + "}}";
            var replacement = htmlEncode && !string.Equals(pair.Key, "Content", StringComparison.OrdinalIgnoreCase)
                ? WebUtility.HtmlEncode(pair.Value)
                : pair.Value;
            result = result.Replace(token, replacement, StringComparison.OrdinalIgnoreCase);
        }
        return result;
    }
}
