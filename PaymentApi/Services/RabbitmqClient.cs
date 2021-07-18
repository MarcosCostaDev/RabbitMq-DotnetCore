using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PaymentApi.Models;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentApi.Services
{
    public interface IRabbitmqClient
    {
        void SendPayment(CardPayment cardPayment);
        void SendPurchaseOrder(PurchaseOrder purchaseOrder);
    }
    public class RabbitmqClient : IRabbitmqClient, IDisposable
    {
        private const string ExchangeName = "Topic_Exchange";
        private const string CardPaymentQueueName = "CardPaymentTopic_Queue";
        private const string PurchaseOrderQueueName = "PurchaseOrder_Queue";
        private const string AllQueueName = "AllTopic_Queue";
        private readonly ConnectionFactory _connectionFactory;
        private readonly IConnection _connection;
        private IModel _model;

        public RabbitmqClient(IConfiguration configuration)
        {
            _connectionFactory = new ConnectionFactory
            {
                Uri = new Uri(configuration.GetConnectionString("rabbitmq"))
            };
            _connection = _connectionFactory.CreateConnection();
            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
            _model = _connection.CreateModel();

            _model.ExchangeDeclare(ExchangeName, "topic");

            _model.QueueDeclare(CardPaymentQueueName, true, false, false, null);
            _model.QueueDeclare(PurchaseOrderQueueName, true, false, false, null);
            _model.QueueDeclare(AllQueueName, true, false, false, null);

            _model.QueueBind(CardPaymentQueueName, ExchangeName, "payment.card");
            _model.QueueBind(PurchaseOrderQueueName, ExchangeName, "payment.purchaseorder");
            _model.QueueBind(AllQueueName, ExchangeName, "payment.*");
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {

        }

        public void Dispose()
        {
        }

        public void SendPayment(CardPayment cardPayment)
        {
            SendMessage(JsonConvert.SerializeObject(cardPayment), "payment.card");
        }

        public void SendPurchaseOrder(PurchaseOrder purchaseOrder)
        {
            SendMessage(JsonConvert.SerializeObject(purchaseOrder), "payment.purchaseorder");
        }

        private void SendMessage(string message, string routingKey)
        {
            var props = _model.CreateBasicProperties();
            props.ContentType = "application/json";
            _model.BasicPublish(ExchangeName, routingKey, props, Encoding.ASCII.GetBytes(message));
        }

    }
}
