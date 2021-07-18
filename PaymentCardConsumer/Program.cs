using System;

namespace PaymentCardConsumer
{
    class Program
    {
        static void Main(string[] args)
        {
            using var client = new RabbitmqConsumer();
            client.CreateConnection();
            client.ProcessMessages();

        }
    }
}
