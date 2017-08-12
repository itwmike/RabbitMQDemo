using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System; 

namespace RabbitMQDemo.Customer
{
    public class FanoutExchange
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

            var exchangeName = "FanoutTest1";
            var queueName1 = "FanoutQueue1";
            var queueName2 = "FanoutQueue2";
            var durable = true;//约定使用持久化
            var noack = false;//消息手动确认,否则消费者在接收到消息后会自动应答

            IConnection conn = factory.CreateConnection();
            IModel model = conn.CreateModel();
            //我们在消费端 从新进行一次 队列和交换机的绑定 ，防止 因为消费端在生产端 之前运行的 问题。
            model.ExchangeDeclare(exchangeName, ExchangeType.Fanout, durable);
            //声明2个队列和Fanout 交换机绑定
            model.QueueDeclare(queueName1, durable, false, false, null);
            model.QueueBind(queueName1, exchangeName, "", null);
            model.QueueDeclare(queueName2, durable, false, false, null);
            model.QueueBind(queueName2, exchangeName, "", null);


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
            #endregion

        }
    }
}
