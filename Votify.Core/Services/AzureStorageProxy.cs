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
           
            if (contenidoArchivo == null || contenidoArchivo.Length == 0)
                throw new Exception("Archivo vacío");

            if (string.IsNullOrWhiteSpace(nombreArchivo))
                throw new Exception("Nombre de archivo no válido");

            var extensionesPermitidas = new[]
            {
                ".jpg", ".jpeg", ".png", ".txt", ".pdf", ".doc", ".docx", ".zip", ".pptx", ".ppt"
            };

            var ext = Path.GetExtension(nombreArchivo).ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(ext) || !extensionesPermitidas.Contains(ext))
                throw new Exception("Tipo de archivo no permitido");

            //delega al real
            return await _realService.SubirArchivoAsync(contenidoArchivo, nombreArchivo);
        }

        public async Task EliminarArchivoAsync(string url)
        {
            // ocontrol extra
            if (string.IsNullOrEmpty(url))
                return;

            await _realService.EliminarArchivoAsync(url);
        }
    }
}
