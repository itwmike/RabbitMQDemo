using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQDemo.Producer
{
    public class DirectExchange
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

            var exchangeName = "DirectTest1";
            var queueName = "DirectQueue1";
            var routingKey = "DirectQueue1";
            var durable = true;//约定使用持久化
            var queueArgs = new Dictionary<string, object>();

            IConnection conn = factory.CreateConnection();
            IModel model = conn.CreateModel();

            model.ExchangeDeclare(exchangeName, ExchangeType.Direct, durable);

            //queueArgs.Add("x-max-priority", 10);//设置队列的最大优先级，

            model.QueueDeclare(queueName, durable, false, false, queueArgs);
            model.QueueBind(queueName, exchangeName, routingKey, null);

            var properties = model.CreateBasicProperties();
            properties.Persistent = true;//设置消息的持久化
            //properties.Priority = 9;//可以设置消息的优先级

            for (int i = 1; i < 20; i++)
            {
                byte[] messageBodyBytes = System.Text.Encoding.UTF8.GetBytes("Hello limitcode :" + i);
                model.BasicPublish(exchangeName, routingKey, properties, messageBodyBytes);//推送消息
                Console.WriteLine("已发送 {0} 条消息", i);
                System.Threading.Thread.Sleep(3000);
            }

        }
    }
}
