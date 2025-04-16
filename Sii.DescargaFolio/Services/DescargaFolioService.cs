using System.Net.Http.Headers;
using Sii.DescargaFolio.Helper;

namespace Sii.DescargaFolio.Services;

public class DescargaFolioService : IDescargaFolioService
{
    private const string UrlReObtener = "cvc_cgi/dte/rf_reobtencion3_folios";
    private const string UrlDescargar = "cvc_cgi/dte/rf_genera_archivo";
    private const string InputSelector = "input[type='text'],input[type='hidden'],select";
    private const string UrlAuth = "https://palena.sii.cl/cgi_dte/UPL/DTEauth?1";
    private readonly IHttpClientFactory _factory;
    private Dictionary<string, object>? _inputsText;
    private readonly SiiAuthenticator _authenticator;
    private const string _clientName = "SII";

    public DescargaFolioService(IHttpClientFactory factory, SiiAuthenticator authenticator)
    {
        _factory = factory;
        _authenticator = authenticator;
    }

    public async Task AuthClient(CancellationToken token = default)
    {
        try
        {
            await _authenticator.AutenticarAsync(UrlAuth);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error en la autenticación: {ex.Message}", ex);
        }
    }

    public async Task PrepareFile(
        string rutEmisor,
        int tipoDoc,
        int cantidad,
        DateTime fecha,
        int folioIni,
        int folioFin,
        CancellationToken token = default
    )
    {
        try
        {
            string[] rutParts = rutEmisor.Split('-');
            List<KeyValuePair<string, string>> formData =
            [
                new("RUT_EMP", rutParts[0]),
                new("DV_EMP", rutParts[1]),
                new("COD_DOCTO", tipoDoc.ToString()),
                new("FOLIO_INI", folioIni.ToString()),
                new("FOLIO_FIN", folioFin.ToString()),
                new("CANT_DOCTOS", cantidad.ToString()),
                new("DIA", fecha.Day.ToString()),
                new("MES", fecha.Month.ToString()),
                new("ANO", fecha.Year.ToString()),
            ];

            using HttpResponseMessage response = await SendAsync(
                new HttpRequestMessage(HttpMethod.Post, UrlReObtener)
                {
                    Content = new FormUrlEncodedContent(formData),
                },
                token
            );

            _inputsText = await response.GetFromDom(InputSelector);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error en el proceso de descarga de folios: {ex.Message}", ex);
        }
    }

    public async Task<string> DownloadFile(CancellationToken token = default!)
    {
        try
        {
            string[] keys = new[]
            {
                "RUT_EMP",
                "DV_EMP",
                "COD_DOCTO",
                "FOLIO_INI",
                "FOLIO_FIN",
                "FECHA",
            };
            List<KeyValuePair<string, string>> formData = keys.Select(BuildFormField).ToList();

            using HttpResponseMessage response = await SendAsync(
                new HttpRequestMessage(HttpMethod.Post, UrlDescargar)
                {
                    Content = new FormUrlEncodedContent(formData),
                },
                token
            );

            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/xml");
            return await response.Content.ReadAsStringAsync(token);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al descargar el archivo CAF: {ex.Message}", ex);
        }
    }

    private async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage message,
        CancellationToken token
    )
    {
        try
        {
            HttpClient httpClient = _factory.CreateClient(_clientName);
            HttpResponseMessage response = await httpClient.SendAsync(message, token);
            return response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"Error de red al enviar la solicitud HTTP: {ex.Message}", ex);
        }
    }

    private KeyValuePair<string, string> BuildFormField(string key)
    {
        return new KeyValuePair<string, string>(
            key,
            _inputsText?.GetValueOrDefault(key)?.ToString() ?? string.Empty
        );
    }
}
