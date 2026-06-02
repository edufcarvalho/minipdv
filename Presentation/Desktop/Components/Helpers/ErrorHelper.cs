using System.Text.Json;

namespace minipdv.Presentation.Desktop.Components.Helpers;

public static class ErrorHelper
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static async Task<string> ExtractAsync(HttpResponseMessage response)
    {
        var body = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrWhiteSpace(body))
            return "Erro desconhecido";

        try
        {
            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;

            if (root.TryGetProperty("error", out var errorProp) && errorProp.ValueKind == JsonValueKind.String)
                return errorProp.GetString() ?? body;

            if (root.TryGetProperty("errors", out var errorsProp))
            {
                if (errorsProp.ValueKind == JsonValueKind.Array)
                {
                    var messages = new List<string>();
                    foreach (var item in errorsProp.EnumerateArray())
                    {
                        if (item.TryGetProperty("errorMessage", out var em))
                            messages.Add(em.GetString() ?? "");
                        else if (item.TryGetProperty("ErrorMessage", out var em2))
                            messages.Add(em2.GetString() ?? "");
                    }
                    if (messages.Count > 0)
                        return string.Join(Environment.NewLine, messages.Where(m => !string.IsNullOrEmpty(m)));
                }
                else if (errorsProp.ValueKind == JsonValueKind.Object)
                {
                    var messages = new List<string>();
                    foreach (var prop in errorsProp.EnumerateObject())
                    {
                        if (prop.Value.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var item in prop.Value.EnumerateArray())
                            {
                                if (item.ValueKind == JsonValueKind.String)
                                    messages.Add(item.GetString() ?? "");
                            }
                        }
                    }
                    if (messages.Count > 0)
                        return string.Join(Environment.NewLine, messages.Where(m => !string.IsNullOrEmpty(m)));
                }
            }
        }
        catch
        {
        }

        return body;
    }
}
