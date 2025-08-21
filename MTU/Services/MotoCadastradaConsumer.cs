using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using MTU.DTO.Events;
using MTU.Data;
using MTU.Model;

namespace MTU.Services
{
    public class MotoCadastradaConsumer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConnection _connection;
        private IModel _channel;
        
        public MotoCadastradaConsumer(IServiceProvider serviceProvider, IConnection connection)
        {
            _serviceProvider = serviceProvider;
            _connection = connection;
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(
                queue: "motos_cadastradas",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var mensagem = Encoding.UTF8.GetString(body);

                var evento = JsonSerializer.Deserialize<MotoCadastradaEvent>(mensagem);

                if (evento != null && evento.Ano == 2024)
                {
                    using var scope = _serviceProvider.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    var notificacao = new NotificacaoMoto
                    {
                        MotoId = evento.Id,
                        Modelo = evento.Modelo,
                        Ano = evento.Ano,
                        Placa = evento.Placa,
                        DataRecebida = DateTime.UtcNow
                    };

                    context.NotificacoesMotos.Add(notificacao);
                    await context.SaveChangesAsync();
                }

                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume(
                queue: "motos_cadastradas",
                autoAck: false,
                consumer: consumer
            );

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            base.Dispose();
        }
    }
}