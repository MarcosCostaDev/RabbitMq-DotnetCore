using System;

namespace PaymentCardConsumer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Listening for Topic <payment.cardpayment>");
            Console.WriteLine("-----------------------------------------");
            Console.WriteLine();

            using var client = new RabbitmqConsumer();
            client.CreateConnection();
            client.ProcessMessages();
            Console.ReadLine();
        }
    }
}
