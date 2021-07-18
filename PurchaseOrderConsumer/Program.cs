using System;

namespace PurchaseOrderConsumer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Listening for Topic <payment.purchaseorder>");
            Console.WriteLine("------------------------------------------");
            Console.WriteLine();
            
            using var client = new RabbitmqConsumer();
            client.CreateConnection();
            client.ProcessMessages();

            Console.ReadLine();
        }
    }
}
