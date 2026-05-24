namespace Votify.Core.Observer
{
    public class EventoNotificador : Subject
    {
        private readonly List<Observer> _observers = new();

        public void suscribir(Observer observer) => _observers.Add(observer);

        public void desuscribir(Observer observer) => _observers.Remove(observer);

        // Cumple el contrato de Subject; el asunto queda genérico
        public void notificar(string mensaje) => notificar("Notificación de Votify", mensaje);

        public void notificar(string asunto, string mensaje)
        {
            var notification = new NotificationEvent(string.Empty, asunto, mensaje);
            foreach (var observer in _observers)
                observer.Update(notification);
        }

        public async Task notificarAsync(string asunto, string mensaje)
        {
            var notification = new NotificationEvent(string.Empty, asunto, mensaje);
            foreach (var observer in _observers)
                await observer.UpdateAsync(notification);
        }
    }
}
