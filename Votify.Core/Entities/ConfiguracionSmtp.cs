namespace Votify.Core.Entities
{
    public class ConfiguracionSmtp
    {
        public Guid Id { get; set; }
        public string Host { get; set; } = "smtp.gmail.com";
        public int Port { get; set; } = 587;
        public string UserName { get; set; } = "";
        public string Password { get; set; } = "";
        public string FromName { get; set; } = "Votify";
    }
}
