using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectPaymentCardConsumer
{
    public class RabbitmqConsumer : IDisposable
    {
        private const string connectionString = "amqp://guest:guest@localhost:5672/";
        private ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _channel;
        private static Random _rnd;
        private EventingBasicConsumer _consumer;

        public void CreateConnection()
        {
            _connectionFactory = new ConnectionFactory
            {
                Uri = new Uri(connectionString)
            };
            _connection = _connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare("rpc_queue", false, false, false, null);
            _channel.BasicQos(0, 1, false);
            _consumer = new EventingBasicConsumer(_channel);
            _channel.BasicConsume("rpc_queue", false, _consumer);
            _rnd = new Random();

        }

        private string MakePayment(BasicGetResult e)
        {
            var content = Encoding.UTF8.GetString(e.Body.ToArray());

            var message = JsonConvert.DeserializeObject<CardPayment>(content);

            var response = _rnd.Next(1000, 100000000).ToString(CultureInfo.InvariantCulture);
            Console.WriteLine("Payment -  {0} : £{1} : Auth Code <{2}> ", message.CardNumber, message.Amount, response);

            return response;
        }

        private void GetMessageFromQueue()
        {
            string response = null;
            var ea = _consumer.Model.BasicGet("rpc_queue", false);
            if (ea == null) return;
            var props = ea.BasicProperties;
            var replyProps = _channel.CreateBasicProperties();
            replyProps.CorrelationId = props.CorrelationId;

            Console.WriteLine("----------------------------------------------------------");

            try
            {
                response = MakePayment(ea);
                Console.WriteLine("Correlation ID = {0}", props.CorrelationId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(" ERROR : " + ex.Message);
                response = "";
            }
            finally
            {
                if (response != null)
                {
                    var responseBytes = Encoding.UTF8.GetBytes(response);
                    _channel.BasicPublish("", props.ReplyTo, replyProps, responseBytes);
                }
                _channel.BasicAck(ea.DeliveryTag, false);
            }

            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine("");
        }


        public void Dispose()
        {
            _connection?.Dispose();
            _channel?.Dispose();

        }

        public void ProcessMessages()
        {
            while (true)
            {
                GetMessageFromQueue();
            }
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
