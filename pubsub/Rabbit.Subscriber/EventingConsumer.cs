using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace ReliableConsumer
{
    public class EventingConsumer : EventingBasicConsumer
	{
        public EventingConsumer(IModel channel) : base(channel) {
            Received += ConsumeMessage;
        }
            
        static void ConsumeMessage(object sender, BasicDeliverEventArgs ea)
        {
            IModel channel = sender as IModel;
            try
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                var routingKey = ea.RoutingKey;

                Console.WriteLine("EventingConsumer: received '{0}':'{1}'", routingKey, message);
                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);

            }
            catch (Exception e)
            {
                channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                Console.WriteLine("     Exception in reliable consumer: " + e.ToString());
            }
        }
	}

}
