using System;

namespace AccountsAuditConsumer
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("Listening for Topic <payment.*>");
            Console.WriteLine("------------------------------");
            Console.WriteLine();

            using var client = new RabbitmqConsumer();
            client.CreateConnection();
            client.ProcessMessages();
            Console.ReadLine();
        }
    }
}
