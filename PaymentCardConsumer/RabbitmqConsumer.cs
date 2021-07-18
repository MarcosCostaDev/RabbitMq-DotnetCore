using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PaymentCardConsumer
{
    public class RabbitmqConsumer : IDisposable
    {
        private const string ExchangeName = "Topic_Exchange";
        private const string CardPaymentQueueName = "CardPaymentTopic_Queue";
        private const string connectionString = "amqp://guest:guest@localhost:5672/";
        private ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _channel;
        public void CreateConnection()
        {
            _connectionFactory = new ConnectionFactory
            {
                Uri = new Uri(connectionString)
            };

        }

        public void Dispose()
        {
            _connection?.Dispose();
            _channel?.Dispose();

        }

        public void ProcessMessages()
        {
            _connection = _connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(ExchangeName, "topic");
            _channel.QueueDeclare(CardPaymentQueueName, true, false, false, null);
            _channel.QueueBind(CardPaymentQueueName, ExchangeName, "payment.cardpayment");

            _channel.BasicQos(0, 1, false);
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += Consumer_Received;
            _channel.BasicConsume(CardPaymentQueueName, false, consumer);
        }

        private void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            try
            {
                var content = Encoding.UTF8.GetString(e.Body.ToArray());
                var message = JsonConvert.DeserializeObject<CardPayment>(content);

                Console.WriteLine("--- Payment - Routing Key <{0}> : {1} : {2}", e.RoutingKey, message.CardNumber, message.Amount);

                _channel.BasicAck(e.DeliveryTag, false);
            }
            catch (Exception)
            {
                _channel.BasicReject(e.DeliveryTag, true);
            }
        }
    }
}
