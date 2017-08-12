using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RabbitMQDemo.Customer
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Threading.Tasks.Task.Factory.StartNew(() => {
                new DirectExchange().Start();
                //new FanoutExchange().Start();
                //new TopicExchange().Start();
            });
            Console.ReadKey();
        }
    }
}
