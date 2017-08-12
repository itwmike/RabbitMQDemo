using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RabbitMQDemo.Customer
{
    public class FanoutExchange
    {
        public void Start()
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.UserName = "guest";
            factory.Password = "guest";
            factory.VirtualHost = "/";//使用默认虚拟机
            factory.HostName = "127.0.0.1";

            //上面的4中配置方式也可以使用下面这一种
            //factory.Uri = "amqp://user:pass@hostName:port/vhost";

            var exchangeName = "FanoutTest1";
            var queueName1 = "FanoutQueue1";
            var queueName2 = "FanoutQueue2";
            var durable = true;//约定使用持久化

            IConnection conn = factory.CreateConnection();
            IModel model = conn.CreateModel();
            model.ExchangeDeclare(exchangeName, ExchangeType.Fanout, durable);
            //声明2个队列和Fanout 交换机绑定
            model.QueueDeclare(queueName1, durable, false, false, null);
            model.QueueBind(queueName1, exchangeName, "", null);
            model.QueueDeclare(queueName2, durable, false, false, null);
            model.QueueBind(queueName2, exchangeName, "", null);

            var properties = model.CreateBasicProperties();
            properties.Persistent = true;//设置消息的持久化

            for (int i = 1; i < 20; i++)
            {
                byte[] messageBodyBytes = System.Text.Encoding.UTF8.GetBytes("Hello limitcode :" + i);
                model.BasicPublish(exchangeName, "", properties, messageBodyBytes);//推送消息
                Console.WriteLine("已发送 {0} 条消息", i);
                System.Threading.Thread.Sleep(3000);
            }
        }
    }
}
