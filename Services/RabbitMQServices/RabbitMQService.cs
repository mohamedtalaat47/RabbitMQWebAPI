using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQWebAPI.Services.RabbitMQServices
{
    public class RabbitMQService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMQService(IConfiguration configuration)
        {
            var factory = new ConnectionFactory()
            {
                HostName = configuration["RabbitMQ:HostName"],
                UserName = configuration["RabbitMQ:UserName"],
                Password = configuration["RabbitMQ:Password"]
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        public void PublishMessage(string message, string queueName)
        {
            _channel.QueueDeclare(
                queue: queueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(
                exchange: "",
                routingKey: queueName,
                basicProperties: null,
                body: body
            );
        }

        public void ConsumeMessages(string queueName, Action<string> handleMessage)
        {
            _channel.QueueDeclare(
                queue: queueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                handleMessage(message);
            };

            _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
        }

        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
        }
    }
}
