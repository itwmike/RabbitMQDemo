using RabbitMQ.Client;
using System; 

namespace RabbitMQDemo.Producer
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

            var routingKey1 = "www.limitcode.com";
            var routingKey2 = "www.limitcode.com.cn";
            var routingKey3 = "api.limitcode.com";

            var durable = true;//约定使用持久化

            IConnection conn = factory.CreateConnection();
            IModel model = conn.CreateModel();

            model.ExchangeDeclare(exchangeName, ExchangeType.Direct, durable);
            model.QueueDeclare(queueName1, durable, false, false, null);
            model.QueueBind(queueName1, exchangeName, bindingKey1, null);
            model.QueueDeclare(queueName2, durable, false, false, null);
            model.QueueBind(queueName2, exchangeName, bindingKey2, null);
            model.QueueDeclare(queueName3, durable, false, false, null);
            model.QueueBind(queueName3, exchangeName, bindingKey3, null);

            var properties = model.CreateBasicProperties();
            properties.Persistent = true;//设置消息的持久化

            for (int i = 1; i < 20; i++)
            {
                byte[] messageBodyBytes = System.Text.Encoding.UTF8.GetBytes("Hello limitcode :" + i);
                    
                model.BasicPublish(exchangeName, routingKey1, properties, messageBodyBytes);//推送消息
                model.BasicPublish(exchangeName, routingKey2, properties, messageBodyBytes);//推送消息
                model.BasicPublish(exchangeName, routingKey3, properties, messageBodyBytes);//推送消息

                Console.WriteLine("已发送 {0} 条消息", i);
                System.Threading.Thread.Sleep(3000);
            }
        }
    }
}
