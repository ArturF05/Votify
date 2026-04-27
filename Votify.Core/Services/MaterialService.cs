using Votify.Core.DTOs;
using Votify.Core.Entities;
using Votify.Core.Interfaces;

namespace Votify.Core.Services
{
    public class MaterialService : IMaterialService
    {
        private readonly IMaterialRepository _materialRepository;
        private readonly IAzureStorage _azure;

        public MaterialService(IMaterialRepository materialRepository, IAzureStorage azure)
        {
            _materialRepository = materialRepository;
            _azure = azure;
        }

        public async Task<List<ProyectoConMaterialesDto>> ObtenerProyectosUsuario(Guid usuarioId)
        {
            var materialesDb = await _materialRepository.ObtenerPorUsuarioAsync(usuarioId);

            // Agrupamos por ProyectoId e intentamos sacar el título
            return materialesDb
                .GroupBy(m => new { m.ProyectoId, m.Proyecto?.Titulo })
                .Select(g => new ProyectoConMaterialesDto
                {
                    ProyectoId = g.Key.ProyectoId,
                    Titulo = g.Key.Titulo ?? "Proyecto sin título",
                    Materiales = g.Select(m => new MaterialDto
                    {
                        Id = m.Id,
                        Nombre = m.Nombre,
                        Tipo = m.Tipo,
                        Url = m.Url,
                        FechaSubida = m.FechaSubida
                    }).ToList()
                })
                .ToList();
        }

        public async Task GuardarMateriales(Guid proyectoId, List<MaterialDto> materiales)
        {
            foreach (var mat in materiales.Where(m => m.EsNuevo))
            {
                string urlFinal = mat.Valor ?? "";

                if ((mat.Tipo == "Imagen" || mat.Tipo == "Archivo") && mat.ArchivoBytes != null)
                {
                    var extensionesPermitidas = new[] { ".jpg",".jpeg", ".png", ".pdf", ".doc", ".docx", ".zip", ".tgz", ".pptx", ".ppt" };

                    var ext = Path.GetExtension(mat.NombreArchivo ?? "").ToLower();

                    if (!extensionesPermitidas.Contains(ext))
                    {
                        throw new Exception("Tipo de archivo no permitido, sólo se permiten archivos con tipo .jpg, .png, .pdf, .doc, .docx, .zip, .tgz, .pptx o .ppt");
                    }
                    using var stream = new MemoryStream(mat.ArchivoBytes);
                    urlFinal = await _azure.SubirArchivoAsync(stream, mat.NombreArchivo!);
                }

                var material = new Material
                {
                    Nombre = mat.Nombre,
                    Tipo = mat.Tipo,
                    Url = urlFinal,
                    ProyectoId = proyectoId,
                    FechaSubida = DateTime.UtcNow
                };

                await _materialRepository.CrearAsync(material);
            }

            await _materialRepository.GuardarCambiosAsync();
        }

        public async Task EliminarMaterial(Guid materialId)
        {
            var material = await _materialRepository.ObtenerPorIdAsync(materialId);

            if (material == null) return;

            if ((material.Tipo == "Imagen" || material.Tipo == "Archivo")
                && !string.IsNullOrEmpty(material.Url))
            {
                await _azure.EliminarArchivoAsync(material.Url);
            }


            await _materialRepository.EliminarAsync(material);
            await _materialRepository.GuardarCambiosAsync();
        }
    }
}