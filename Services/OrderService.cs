using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using OrderService.Models;

namespace OrderService.Services
{
    public class OrderPublisher
    {
        private readonly string _hostname = "localhost";
        private readonly string _queueName = "orderQueue";

        public void PublishOrder(Order order)
        {
            var factory = new ConnectionFactory() { HostName = _hostname };

            // FIX: Yeh line barobar likh
            using var connection = factory.CreateConnection(); 
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: _queueName,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var message = JsonSerializer.Serialize(order);
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "",
                                 routingKey: _queueName,
                                 basicProperties: null,
                                 body: body);

            Console.WriteLine($"Order Published: {message}");
        }
    }
}
