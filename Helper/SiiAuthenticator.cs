using System.Net;
using System.Text.Json;

namespace Sii.DescargaFolio.Helper;

public class SiiAuthenticator
{
    private readonly IHttpClientFactory _httpClientFactory;
    private const string UrlAuth = "https://herculesr.sii.cl/cgi_AUT2000/CAutInicio.cgi";
    private const string UrlCheckEstado = "https://zeusr.sii.cl/cgi_AUT2000/AutTknData.cgi";

    private static bool _isConnected;
    private static DateTime? _ultimaAutenticacion;
    private static readonly TimeSpan _duracionSesion = TimeSpan.FromHours(2);

    public SiiAuthenticator(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task AutenticarAsync(string referenciaUrl)
    {
        if (await IsConnectedAsync())
            return;

        try
        {
            HttpClient client = _httpClientFactory.CreateClient("SII");
            HttpResponseMessage response = await client.PostAsync(
                UrlAuth,
                new FormUrlEncodedContent(
                    [new KeyValuePair<string, string>("referencia", referenciaUrl)]
                )
            );

            if (response.StatusCode == HttpStatusCode.Found)
            {
                throw new Exception(
                    "El SII respondió con redirección (302). Puede deberse a un problema con el certificado o sesión expirada."
                );
            }

            if (!response.IsSuccessStatusCode)
            {
                string msg = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error HTTP {response.StatusCode}: {msg}");
            }

            _isConnected = true;
            _ultimaAutenticacion = DateTime.Now; // Hora local del sistema que ejecuta la app
        }
        catch (Exception ex)
        {
            _isConnected = false;
            throw new Exception($"Fallo la autenticación con el SII: {ex.Message}", ex);
        }
    }

    private async Task<bool> IsConnectedAsync()
    {
        if (
            _ultimaAutenticacion.HasValue
            && DateTime.Now - _ultimaAutenticacion.Value < _duracionSesion
        )
        {
            return true;
        }

        try
        {
            HttpClient client = _httpClientFactory.CreateClient("SII");
            HttpResponseMessage response = await client.GetAsync(UrlCheckEstado);

            if (!response.IsSuccessStatusCode)
            {
                string body = await response.Content.ReadAsStringAsync();
                throw new Exception(
                    $"Error al verificar sesión: {(int)response.StatusCode} {response.ReasonPhrase}\n{body}"
                );
            }

            string rawJson = await response.Content.ReadAsStringAsync();
            string cleanedJson = rawJson.Replace("my_callback(", "").Replace(")", "");
            Dictionary<string, JsonElement>? json = JsonSerializer.Deserialize<
                Dictionary<string, JsonElement>
            >(cleanedJson);

            if (json is null || !json.TryGetValue("estado", out JsonElement estadoElement))
                return false;

            _isConnected = estadoElement.GetInt32() == 0;

            if (_isConnected)
                _ultimaAutenticacion = DateTime.Now; // Hora local del sistema que ejecuta la app

            return _isConnected;
        }
        catch (Exception ex)
        {
            _isConnected = false;
            throw new Exception("Error al consultar estado de sesión con el SII.", ex);
        }
    }
}
