using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RabbitMQDemo.Customer
{
   public  class TopicExchange
    {
        public void Start()
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.UserName = "guest";
            factory.Password = "guest";
            factory.VirtualHost = "/";
            factory.HostName = "127.0.0.1";

            //上面的4中配置方式也可以使用下面这一种
            //factory.Uri = "amqp://user:pass@hostName:port/vhost";

            var exchangeName = "TopicTest1";
            var queueName1 = "TopicQueue1";
            var queueName2 = "TopicQueue2";
            var queueName3 = "TopicQueue3";

            var bindingKey1 = "*.limitcode.*";
            var bindingKey2 = "*.limitcode.#";
            var bindingKey3 = "api.limitcode.*";

            var durable = true;//约定使用持久化
            var noack = false;//消息手动确认,否则消费者在接收到消息后会自动应答

            IConnection conn = factory.CreateConnection();
            IModel model = conn.CreateModel();

            model.ExchangeDeclare(exchangeName, ExchangeType.Direct, durable);
            model.QueueDeclare(queueName1, durable, false, false, null);
            model.QueueBind(queueName1, exchangeName, bindingKey1, null);
            model.QueueDeclare(queueName2, durable, false, false, null);
            model.QueueBind(queueName2, exchangeName, bindingKey2, null);
            model.QueueDeclare(queueName3, durable, false, false, null);
            model.QueueBind(queueName3, exchangeName, bindingKey3, null);

            #region 通过事件的形式，如果队列中有消息，则执行事件。建议采用这种方式。
            model.BasicQos(0, 1, false);//设置一个消费者在同一时间只处理一个消息，这个rabbitmq 就会将消息公平分发
            var consumer = new EventingBasicConsumer(model);
            consumer.Received += (ch, ea) =>
            {
                var content = System.Text.Encoding.UTF8.GetString(ea.Body);
                Console.WriteLine("获取到消息：{0}", content);
                model.BasicAck(ea.DeliveryTag, false);
            };
            model.BasicConsume(queueName1, noack, consumer);
            model.BasicConsume(queueName2, noack, consumer);
            model.BasicConsume(queueName3, noack, consumer);
            #endregion
        }
    }
}
