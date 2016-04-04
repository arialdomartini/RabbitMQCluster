using RabbitMQ.Client;

namespace Rabbit.Publisher
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var hostName = "192.168.99.100"; //args[0];

            var publisher = new Publisher();

            publisher.Run(hostName);
        }
    }
}
