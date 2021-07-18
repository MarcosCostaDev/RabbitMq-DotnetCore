using System;

namespace DirectPaymentCardConsumer
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Direct Payment <Queue rpc_queue>");
            Console.WriteLine("------------------------------------------");
            Console.WriteLine();

            var client = new RabbitmqConsumer();
            client.CreateConnection();
            client.ProcessMessages();
            Console.ReadLine();
        }
    }
}
