namespace Votify.API.DTOs
{
    public class CrearPremioDTO
    {
        public Guid IdCategoriaEvento { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public int Orden { get; set; }
        public int Puesto { get; set; } = 1;
        public List<Guid> IdsVotaciones { get; set; } = new();
        public byte[]? ImagenBytes { get; set; }
        public string? ImagenNombre { get; set; }
    }

    public class EditarPremioDTO
    {
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public int Orden { get; set; }
        public int Puesto { get; set; } = 1;
        public List<Guid>? IdsVotaciones { get; set; }
        public byte[]? ImagenBytes { get; set; }
        public string? ImagenNombre { get; set; }
        public bool QuitarImagen { get; set; }
    }

    public class VincularPremioVotacionesDTO
    {
        public List<Guid> IdsVotaciones { get; set; } = new();
    }
}
