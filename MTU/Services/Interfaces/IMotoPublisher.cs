using MTU.Events;

namespace MTU.Services.Interfaces
{
    public interface IMotoPublisher
    {
        void Publicar(string fila, MotoCadastradaEvent evento);
    }
}
