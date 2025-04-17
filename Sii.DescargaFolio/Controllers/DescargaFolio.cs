using System.Net.Mime;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Sii.DescargaFolio.Helper;
using Sii.DescargaFolio.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace Sii.DescargaFolio.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DescargaFolioController : ControllerBase
{
    private readonly IDescargaFolioService _descargaFolioService;

    public DescargaFolioController(IDescargaFolioService descargaFolioService)
    {
        _descargaFolioService = descargaFolioService;
    }

    [SwaggerOperation(
        Summary = "Obtiene un archivo XML CAF",
        Description = "Obtiene un XML CAF ya pedido con anterioridad."
    )]
    [HttpGet("caf")]
    public async Task<IActionResult> GetCAF(
        [SwaggerParameter("RUT del emisor (ej: 76285488-7)")] [FromQuery] string rut,
        [SwaggerParameter("Tipo de documento (ej: 33 para factura electrónica)")]
        [FromQuery]
            int tipoDoc,
        [SwaggerParameter("Número de folio inicial")] [FromQuery] int folioInicio,
        [SwaggerParameter("Número de folio final")] [FromQuery] int folioFin,
        [SwaggerParameter("Fecha del CAF ya pedido, en formato dd-MM-yyyy")]
        [FromQuery]
            string fecha
    )
    {
        try
        {
            int cantidad = folioFin - folioInicio + 1;
            DateTime fechaParsed = DateTime.ParseExact(fecha, "dd-MM-yyyy", null);

            string xmlContent = await _descargaFolioService
                .Autenticar()
                .PrepararXml(rut, tipoDoc, cantidad, fechaParsed, folioInicio, folioFin)
                .DescargarXml();

            string fileName = $"CAF_{rut}_{tipoDoc}_{folioInicio}_{folioFin}.xml";
            byte[] bytes = Encoding.UTF8.GetBytes(xmlContent);
            return File(bytes, MediaTypeNames.Text.Xml, fileName);
        }
        catch (FormatException)
        {
            return BadRequest("El formato de la fecha debe ser dd-MM-yyyy.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al descargar el archivo CAF: {ex.Message}");
        }
    }
}
