using Votify.Core.Entities;

namespace Votify.Core.Interfaces
{
    public interface IConfiguracionSmtpRepository
    {
        Task<ConfiguracionSmtp?> ObtenerAsync();
        Task GuardarAsync(ConfiguracionSmtp config);
    }
}
