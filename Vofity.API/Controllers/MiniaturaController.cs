using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/miniaturas")]
public class MiniaturaController : ControllerBase // Cambiado a ControllerBase por ser API
{
    private readonly IHttpClientFactory _httpClientFactory;

    public MiniaturaController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet("proxy")]
    public async Task<IActionResult> Proxy([FromQuery] string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return BadRequest("URL no proporcionada");

        // Opcional: Validar que la URL apunte a tu Azure Blob Storage para evitar ataques SSRF
        // ni idea pero lo dejo por si acaso, aunque esto debería ser controlado en el frontend al generar las URLs
        if (!url.Contains("votifydb.blob.core.windows.net")) return Forbid();

        try
        {
            var client = _httpClientFactory.CreateClient();

            // Usamos HttpCompletionOption.ResponseHeadersRead para no esperar a bajar todo el archivo
            var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

            if (!response.IsSuccessStatusCode)

                return StatusCode((int)response.StatusCode, "No se pudo obtener el recurso");

            var contentType = response.Content.Headers.ContentType?.MediaType;

            if (string.IsNullOrEmpty(contentType) || !contentType.StartsWith("image/"))
            {
                contentType = "image/jpeg";
            }
            var stream = await response.Content.ReadAsStreamAsync();

            
            return File(stream, contentType);
        }
        catch (Exception ex)
        {
           
            return StatusCode(500, "Error interno al procesar la imagen");
        }
    }

    
}