namespace Sii.DescargaFolio.Services;

public interface IDescargaFolioService
{
    Task AuthClient(CancellationToken token = default);
    Task<string> DownloadFile(CancellationToken token = default);
    Task PrepareFile(
        string rutEmisor,
        int tipoDoc,
        int cantidad,
        DateTime fecha,
        int folioIni,
        int folioFin,
        CancellationToken token = default
    );
}
