using Votify.Core.Interfaces;

namespace Votify.Core.Services
{
    public class AzureStorageProxy : IAzureStorage
    {
        private readonly IAzureStorage _realService;

        public AzureStorageProxy(IAzureStorage realService)
        {
            _realService = realService;
        }

        public async Task<string> SubirArchivoAsync(Stream contenidoArchivo, string nombreArchivo)
        {
            // 🔐 CONTROL (esto es el Proxy GoF)
            if (contenidoArchivo == null || contenidoArchivo.Length == 0)
                throw new Exception("Archivo vacío");

            var extensionesPermitidas = new[]
            {
                ".jpg", ".jpeg", ".png", ".pdf", ".doc", ".docx", ".zip", ".pptx", ".ppt"
            };

            var ext = Path.GetExtension(nombreArchivo).ToLower();

            if (!extensionesPermitidas.Contains(ext))
                throw new Exception("Tipo de archivo no permitido");

            // 👉 delega al real
            return await _realService.SubirArchivoAsync(contenidoArchivo, nombreArchivo);
        }

        public async Task EliminarArchivoAsync(string url)
        {
            // opcional: control extra
            if (string.IsNullOrEmpty(url))
                return;

            await _realService.EliminarArchivoAsync(url);
        }
    }
}