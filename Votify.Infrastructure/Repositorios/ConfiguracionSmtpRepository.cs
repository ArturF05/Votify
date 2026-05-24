using Microsoft.EntityFrameworkCore;
using Votify.Core.Entities;
using Votify.Core.Interfaces;
using Votify.Infrastructure.Data;

namespace Votify.Infrastructure.Repositorios
{
    public class ConfiguracionSmtpRepository : IConfiguracionSmtpRepository
    {
        private readonly AppDbContext _context;

        public ConfiguracionSmtpRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ConfiguracionSmtp?> ObtenerAsync() =>
            await _context.ConfiguracionSmtp.FirstOrDefaultAsync();

        public async Task GuardarAsync(ConfiguracionSmtp config)
        {
            var existente = await _context.ConfiguracionSmtp.FirstOrDefaultAsync();
            if (existente == null)
            {
                config.Id = Guid.NewGuid();
                _context.ConfiguracionSmtp.Add(config);
            }
            else
            {
                existente.Host = config.Host;
                existente.Port = config.Port;
                existente.UserName = config.UserName;
                existente.Password = config.Password;
                existente.FromName = config.FromName;
            }
            await _context.SaveChangesAsync();
        }
    }
}
