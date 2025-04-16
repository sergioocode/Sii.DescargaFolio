using Sii.DescargaFolio.Services;

namespace Sii.DescargaFolio.Helper;

public static class DescargaFolioExtensions
{
    public static async Task<IDescargaFolioService> Autenticar(this IDescargaFolioService entity)
    {
        await entity.AuthClient();
        return entity;
    }

    public static async Task<IDescargaFolioService> PrepararXml(
        this Task<IDescargaFolioService> entity,
        string rutEmisor,
        int tipoDoc,
        int cantidad,
        DateTime fecha,
        int folioIni,
        int folioFin,
        CancellationToken token = default
    )
    {
        await (await entity).PrepareFile(
            rutEmisor,
            tipoDoc,
            cantidad,
            fecha,
            folioIni,
            folioFin,
            token
        );
        return await entity;
    }

    public static async Task<string> DescargarXml(this Task<IDescargaFolioService> entity)
    {
        return await (await entity).DownloadFile();
    }
}
