using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;

namespace RabbitMQDemo.Customer
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
            var noack = false;//消息手动确认,否则消费者在接收到消息后会自动应答

            IConnection conn = factory.CreateConnection();
            IModel model = conn.CreateModel();
            //我们在消费端 从新进行一次 队列和交换机的绑定 ，防止 因为消费端在生产端 之前运行的 问题。
            model.ExchangeDeclare(exchangeName, ExchangeType.Fanout, durable);
            model.QueueDeclare(queueName, durable, false, false, null);
            model.QueueBind(queueName, exchangeName, routingKey, null);

            #region 这种方式只能一次性获取队列中的所有消息，并不能做到 时时，所以不采取！
            //while (true)
            //{
            //    var result = model.BasicGet(queueName, noack);//从队列总获取消息
            //    if (result != null)
            //    {
            //        var content = System.Text.Encoding.UTF8.GetString(result.Body);
            //        Console.WriteLine("获取到消息：{0}", content);
            //        model.BasicAck(result.DeliveryTag, false);
            //    }
            //    else
            //    {
            //        break;
            //    }
            //}
            #endregion

            #region 通过事件的形式，如果队列中有消息，则执行事件。建议采用这种方式。
            model.BasicQos(0, 1, false);//设置一个消费者在同一时间只处理一个消息，这个rabbitmq 就会将消息公平分发
            var consumer = new EventingBasicConsumer(model);
            consumer.Received += (ch, ea) =>
            {
                var content = System.Text.Encoding.UTF8.GetString(ea.Body);
                Console.WriteLine("获取到消息：{0}", content);
                model.BasicAck(ea.DeliveryTag, false);
            };
            model.BasicConsume(queueName, noack, consumer);
            #endregion

            #region 此方法同样会阻塞进程，所以不建议使用。
            //QueueingBasicConsumer consumer = new QueueingBasicConsumer(model);//定义这个队列的消费者
            //model.BasicQos(0, 1, false);//设置一个消费者在同一时间只处理一个消息，这个rabbitmq 就会将消息公平分发
            //while (true)
            //{
            //    BasicDeliverEventArgs ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();
            //    var content = Encoding.UTF8.GetString(ea.Body);
            //    Console.WriteLine("获取到消息：{0}", content);
            //    model.BasicAck(ea.DeliveryTag, false);//如果是自动应答，不用写这句代码
            //}
            #endregion

        }
    }
}
