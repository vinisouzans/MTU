using System.Text;
using System.Text.Json;
using MTU.Events;
using MTU.Services.Interfaces;
using RabbitMQ.Client;

namespace MTU.Services
{
    public class RabbitMqPublisher : IMotoPublisher
    {
        private readonly ConnectionFactory _factory;

        public RabbitMqPublisher()
        {
            _factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };
        }

        public void Publicar(string fila, MotoCadastradaEvent evento)
        {
            using var connection = _factory.CreateConnection();
            using var channel = connection.CreateModel();

            // garante que a fila exista
            channel.QueueDeclare(
                queue: fila,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            // serializar o evento em JSON
            var mensagem = JsonSerializer.Serialize(evento);
            var body = Encoding.UTF8.GetBytes(mensagem);

            // publica na fila
            channel.BasicPublish(
                exchange: "",
                routingKey: fila,
                basicProperties: null,
                body: body
            );
        }
    }
}
