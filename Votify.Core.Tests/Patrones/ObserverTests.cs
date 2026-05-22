using Moq;
using Votify.Core.Observer;

namespace Votify.Core.Tests.Patrones
{
    [TestClass]
    public class ObserverTests
    {
        [TestMethod]
        public void Notificar_ObserverSuscripto_RecibeEmailConDestinatarioCorrecto()
        {
            var mockSender = new Mock<IEmailSender>();
            var observer = new EmailObserver("test@test.com", mockSender.Object);
            var notificador = new EventoNotificador();

            notificador.suscribir(observer);
            notificador.notificar("Votación abierta", "El evento ha comenzado.");

            mockSender.Verify(
                s => s.Send(It.Is<NotificationEvent>(n =>
                    n.RecipientEmail == "test@test.com" &&
                    n.Subject == "Votación abierta" &&
                    n.Message == "El evento ha comenzado.")),
                Times.Once);
        }

        [TestMethod]
        public void Notificar_ObserverDesuscripto_NoRecibeEmail()
        {
            var mockSender = new Mock<IEmailSender>();
            var observer = new EmailObserver("test@test.com", mockSender.Object);
            var notificador = new EventoNotificador();

            notificador.suscribir(observer);
            notificador.desuscribir(observer);
            notificador.notificar("Evento", "Mensaje.");

            mockSender.Verify(s => s.Send(It.IsAny<NotificationEvent>()), Times.Never);
        }

        [TestMethod]
        public void Notificar_MultiplesObservers_CadaUnoRecibeEnSuEmail()
        {
            var mockSender1 = new Mock<IEmailSender>();
            var mockSender2 = new Mock<IEmailSender>();
            var notificador = new EventoNotificador();

            notificador.suscribir(new EmailObserver("uno@test.com", mockSender1.Object));
            notificador.suscribir(new EmailObserver("dos@test.com", mockSender2.Object));
            notificador.notificar("Asunto", "Cuerpo.");

            mockSender1.Verify(
                s => s.Send(It.Is<NotificationEvent>(n => n.RecipientEmail == "uno@test.com")),
                Times.Once);
            mockSender2.Verify(
                s => s.Send(It.Is<NotificationEvent>(n => n.RecipientEmail == "dos@test.com")),
                Times.Once);
        }
    }
}
