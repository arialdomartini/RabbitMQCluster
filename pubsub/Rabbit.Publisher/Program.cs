using RabbitMQ.Client;

namespace Rabbit.Publisher
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var hostName = args[0];

            var publisher = new Publisher();

            publisher.Run(hostName);
        }
    }
}
