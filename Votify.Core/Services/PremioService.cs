using Votify.Core.Entities;
using Votify.Core.Interfaces;

namespace Votify.Core.Services
{
    public class PremioService : IPremio
    {
        private readonly IPremioRepository premioRepository;

        public PremioService(IPremioRepository premioRepository)
        {
            this.premioRepository = premioRepository;
        }

        public async Task<Premio> CrearPremioAsync(Guid idCategoriaEvento, string nombre, string? descripcion, int orden, IEnumerable<Guid>? idsVotaciones = null, string? imagenUrl = null, int puesto = 1)
        {
            var categoria = await ObtenerCategoriaValidaAsync(idCategoriaEvento);
            var nombreLimpio = ValidarNombre(nombre);
            var idsVotacionesNormalizados = NormalizarIdsVotaciones(idsVotaciones);

            if (await premioRepository.ExisteNombreEnCategoriaAsync(idCategoriaEvento, nombreLimpio))
                throw new Exception("Ya existe un premio con ese nombre en la categoria.");

            var votaciones = await ValidarVotacionesCompatiblesAsync(categoria, idsVotacionesNormalizados);
            ValidarPuesto(puesto, votaciones);

            var premio = new Premio
            {
                Id = Guid.NewGuid(),
                Id_categoria_evento = idCategoriaEvento,
                Nombre = nombreLimpio,
                Descripcion = NormalizarDescripcion(descripcion),
                ImagenUrl = NormalizarDescripcion(imagenUrl),
                Orden = orden,
                Puesto = puesto,
                PremioVotaciones = idsVotacionesNormalizados
                    .Select(idVotacion => new PremioVotacion
                    {
                        Id_premio = Guid.Empty,
                        Id_votacion = idVotacion
                    })
                    .ToList()
            };

            foreach (var vinculo in premio.PremioVotaciones)
            {
                vinculo.Id_premio = premio.Id;
            }

            await premioRepository.CrearAsync(premio);
            return await premioRepository.ObtenerPorIdAsync(premio.Id) ?? premio;
        }

        public async Task<Premio> EditarPremioAsync(Guid idPremio, string nombre, string? descripcion, int orden, string? imagenUrl, int puesto = 1, IEnumerable<Guid>? idsVotaciones = null)
        {
            var premio = await premioRepository.ObtenerPorIdAsync(idPremio);
            if (premio == null)
                throw new Exception("El premio no existe.");

            var nombreLimpio = ValidarNombre(nombre);
            if (await premioRepository.ExisteNombreEnCategoriaAsync(premio.Id_categoria_evento, nombreLimpio, idPremio))
                throw new Exception("Ya existe un premio con ese nombre en la categoria.");

            var categoria = premio.CategoriaEvento ?? await premioRepository.ObtenerCategoriaEventoAsync(premio.Id_categoria_evento);
            if (categoria == null)
                throw new Exception("La categoria del premio no existe.");

            var sincronizarVotaciones = idsVotaciones != null;
            var idsVotacionesNormalizados = sincronizarVotaciones
                ? NormalizarIdsVotaciones(idsVotaciones)
                : NormalizarIdsVotaciones(premio.PremioVotaciones.Select(pv => pv.Id_votacion));

            var votaciones = await ValidarVotacionesCompatiblesAsync(categoria, idsVotacionesNormalizados);
            ValidarPuesto(puesto, votaciones);

            premio.Nombre = nombreLimpio;
            premio.Descripcion = NormalizarDescripcion(descripcion);
            premio.ImagenUrl = NormalizarDescripcion(imagenUrl);
            premio.Orden = orden;
            premio.Puesto = puesto;

            await premioRepository.ActualizarAsync(premio);

            if (sincronizarVotaciones)
                await premioRepository.SincronizarVinculosAsync(idPremio, idsVotacionesNormalizados);

            return await premioRepository.ObtenerPorIdAsync(premio.Id) ?? premio;
        }

        public async Task<List<Premio>> ObtenerPremiosAsync(Guid? idCategoriaEvento = null)
        {
            return await premioRepository.ObtenerPremiosAsync(NormalizarGuidNullable(idCategoriaEvento));
        }

        public async Task<Premio?> ObtenerPorIdAsync(Guid idPremio)
        {
            if (idPremio == Guid.Empty)
                return null;

            return await premioRepository.ObtenerPorIdAsync(idPremio);
        }

        public async Task<List<PremioVotacion>> ActualizarVotacionesPremioAsync(Guid idPremio, IEnumerable<Guid> idsVotaciones)
        {
            if (idPremio == Guid.Empty)
                throw new Exception("El premio es obligatorio.");

            var premio = await premioRepository.ObtenerPorIdAsync(idPremio);
            if (premio == null)
                throw new Exception("El premio no existe.");

            var categoria = premio.CategoriaEvento ?? await premioRepository.ObtenerCategoriaEventoAsync(premio.Id_categoria_evento);
            if (categoria == null)
                throw new Exception("La categoria del premio no existe.");

            var idsVotacionesNormalizados = NormalizarIdsVotaciones(idsVotaciones);
            var votaciones = await ValidarVotacionesCompatiblesAsync(categoria, idsVotacionesNormalizados);
            ValidarPuesto(premio.Puesto, votaciones);

            return await premioRepository.SincronizarVinculosAsync(idPremio, idsVotacionesNormalizados);
        }

        public async Task<List<Premio>> ObtenerPremiosPorVotacionAsync(Guid idVotacion, Guid? idCategoriaEvento = null)
        {
            if (idVotacion == Guid.Empty)
                return new List<Premio>();

            return await premioRepository.ObtenerPremiosPorVotacionAsync(idVotacion, NormalizarGuidNullable(idCategoriaEvento));
        }

        public async Task EliminarPremioAsync(Guid idPremio)
        {
            var premio = await premioRepository.ObtenerPorIdAsync(idPremio);
            if (premio == null)
                throw new Exception("El premio no existe.");

            await premioRepository.EliminarAsync(premio);
        }

        private async Task<CategoriaEvento> ObtenerCategoriaValidaAsync(Guid idCategoriaEvento)
        {
            if (idCategoriaEvento == Guid.Empty)
                throw new Exception("La categoria del premio es obligatoria.");

            var categoria = await premioRepository.ObtenerCategoriaEventoAsync(idCategoriaEvento);
            if (categoria == null)
                throw new Exception("La categoria del premio no existe.");

            return categoria;
        }

        private async Task<List<Votacion>> ValidarVotacionesCompatiblesAsync(CategoriaEvento categoria, List<Guid> idsVotaciones)
        {
            if (!idsVotaciones.Any())
                throw new Exception("El premio debe estar vinculado al menos a una votacion.");

            var votaciones = new List<Votacion>();

            foreach (var idVotacion in idsVotaciones)
            {
                var votacion = await premioRepository.ObtenerVotacionAsync(idVotacion);
                if (votacion == null)
                    throw new Exception("Una de las votaciones seleccionadas no existe.");

                if (votacion.Id_evento != categoria.Id_evento)
                    throw new Exception("La votacion no pertenece al evento de la categoria del premio.");

                if (votacion.Id_categoria_evento.HasValue &&
                    votacion.Id_categoria_evento.Value != categoria.Id)
                    throw new Exception("La votacion no corresponde a la categoria del premio.");

                if (!votacion.Id_categoria_evento.HasValue &&
                    !await premioRepository.VotacionIncluyeCategoriaAsync(idVotacion, categoria.Id))
                    throw new Exception("La votacion general no tiene criterios para la categoria del premio.");

                votaciones.Add(votacion);
            }

            return votaciones;
        }

        private static void ValidarPuesto(int puesto, IEnumerable<Votacion> votaciones)
        {
            ValidarPuestoBase(puesto);

            foreach (var votacion in votaciones)
            {
                if (votacion.NumProy > 0 && puesto > votacion.NumProy)
                    throw new Exception($"El puesto {puesto} no existe en una de las votaciones seleccionadas.");
            }
        }

        private static void ValidarPuestoBase(int puesto)
        {
            if (puesto < 1)
                throw new Exception("El puesto del premio debe ser mayor que cero.");
        }

        private static string ValidarNombre(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new Exception("El nombre del premio es obligatorio.");

            return nombre.Trim();
        }

        private static string? NormalizarDescripcion(string? descripcion)
        {
            return string.IsNullOrWhiteSpace(descripcion)
                ? null
                : descripcion.Trim();
        }

        private static List<Guid> NormalizarIdsVotaciones(IEnumerable<Guid>? idsVotaciones)
        {
            return idsVotaciones?
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToList() ?? new List<Guid>();
        }

        private static Guid? NormalizarGuidNullable(Guid? id)
        {
            return id.HasValue && id.Value != Guid.Empty
                ? id.Value
                : null;
        }
    }
}
