using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace minipdv.Presentation.Desktop;

public class ApiClient
{
    public static ApiClient Instance { get; } = new();

    private readonly HttpClient _http;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        ReferenceHandler = ReferenceHandler.IgnoreCycles
    };

    public int UserId { get; private set; }
    public string UserName { get; private set; } = "";
    public string UserLogin { get; private set; } = "";
    public string UserTipo { get; private set; } = "";
    public bool IsAuthenticated => !string.IsNullOrEmpty(_token);

    private string? _token;

    private ApiClient()
    {
        var baseUrl = Environment.GetEnvironmentVariable("API_URL") ?? "http://localhost:5000";
        _http = new HttpClient { BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/") };
        _http.Timeout = TimeSpan.FromSeconds(30);
    }

    public void SetSession(string token, int id, string nome, string login, string tipo)
    {
        _token = token;
        UserId = id;
        UserName = nome;
        UserLogin = login;
        UserTipo = tipo;
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public void ClearSession()
    {
        _token = null;
        UserId = 0;
        UserName = "";
        UserLogin = "";
        UserTipo = "";
        _http.DefaultRequestHeaders.Authorization = null;
    }

    public async Task<T?> GetAsync<T>(string endpoint)
    {
        var response = await _http.GetAsync(endpoint);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(json, JsonOptions);
    }

    public async Task<HttpResponseMessage> PostAsync<T>(string endpoint, T data)
    {
        var json = JsonSerializer.Serialize(data, JsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        return await _http.PostAsync(endpoint, content);
    }

    public async Task<HttpResponseMessage> PutAsync<T>(string endpoint, T data)
    {
        var json = JsonSerializer.Serialize(data, JsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        return await _http.PutAsync(endpoint, content);
    }

    public async Task<HttpResponseMessage> DeleteAsync(string endpoint)
    {
        return await _http.DeleteAsync(endpoint);
    }

    public async Task<string?> LoginAsync(string login, string password)
    {
        var response = await PostAsync("api/auth/login", new { login, password, deviceInfo = "WinForms" });
        var json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            try
            {
                var err = JsonSerializer.Deserialize<Dictionary<string, string>>(json, JsonOptions);
                return err?.GetValueOrDefault("message") ?? "Erro de autenticação";
            }
            catch { return "Erro de autenticação"; }
        }

        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;
        var token = root.GetProperty("token").GetString()!;
        var id = root.GetProperty("id").GetInt32();
        var nome = root.GetProperty("nome").GetString()!;
        var login2 = root.GetProperty("login").GetString()!;

        var tipo = DecodeTipoFromToken(token) ?? "Usuario";

        SetSession(token, id, nome, login2, tipo);
        return null;
    }

    private static string? DecodeTipoFromToken(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            return jwt.Claims.FirstOrDefault(c => c.Type == "tipo")?.Value;
        }
        catch { return null; }
    }

    public async Task LogoutAsync()
    {
        try { await _http.PostAsync("api/auth/logout", null); }
        catch { }
        ClearSession();
    }
}
