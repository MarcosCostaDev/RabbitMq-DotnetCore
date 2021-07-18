using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountsAuditConsumer
{
    public class RabbitmqConsumer
    {
        private const string ExchangeName = "Topic_Exchange";
        private const string CardPaymentQueueName = "CardPaymentTopic_Queue";
        private const string connectionString = "amqp://guest:guest@localhost:5672/";
        private static ConnectionFactory _connectionFactory;
        private static IConnection _connection;

        public void CreateConnection()
        {
            _connectionFactory = new ConnectionFactory
            {
                Uri = new Uri(connectionString)
            };
         
        }

        public void ProcessMessages()
        {
            using (_connection = _connectionFactory.CreateConnection())
            {
                using var channel = _connection.CreateModel();

                channel.ExchangeDeclare(ExchangeName, "topic");
                channel.QueueDeclare(CardPaymentQueueName, true, false, false, null);
                channel.QueueBind(CardPaymentQueueName, ExchangeName, "payment.cardpayment");
            }

        }
    }



    
}
