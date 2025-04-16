using System.Net.Mime;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Sii.DescargaFolio.Helper;
using Sii.DescargaFolio.Services;

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

    [HttpGet("caf")]
    public async Task<IActionResult> GetCAF(
        [FromQuery] string rut,
        [FromQuery] int tipoDoc,
        [FromQuery] int folioInicio,
        [FromQuery] int folioFin,
        [FromQuery] string fecha // formato esperado: dd-MM-yyyy
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
