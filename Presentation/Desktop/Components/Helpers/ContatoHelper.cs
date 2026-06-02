using System.Text.Json;
using minipdv.Domain.Entities;

namespace minipdv.Presentation.Desktop.Components.Helpers;

public static class ContatoHelper
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static async Task<int?> CreateOrUpdateAsync(int? contatoId, string email, string telefone)
    {
        if (string.IsNullOrEmpty(email) && string.IsNullOrEmpty(telefone))
            return contatoId;

        if (contatoId.HasValue)
        {
            await ApiClient.Instance.PutAsync($"api/contatos/{contatoId.Value}", new { id = contatoId.Value, email, telefone });
            return contatoId;
        }

        var response = await ApiClient.Instance.PostAsync("api/contatos", new { email, telefone });
        if (!response.IsSuccessStatusCode) return null;

        var json = await response.Content.ReadAsStringAsync();
        var contato = JsonSerializer.Deserialize<Contato>(json, JsonOptions);
        return contato?.Id;
    }
}
