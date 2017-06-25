using Carupano.Messaging.Internal;
using System;
using Xunit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Carupano.Kafka.Tests
{
    public class KafkaEventBusTests : IDisposable
    {
        KafkaEventBus Bus;
        public KafkaEventBusTests()
        {
            var inboundEvents = new List<Route>();
            var outboundEvents = new List<Route>();
            var inboundCommands = new List<Route>();
            var outboundCommands = new List<Route>();
            outboundCommands.Add(new Route("users", typeof(CreateUser)));
            inboundCommands.Add(new Route("users", typeof(CreateUser)));
            var routes = new RouteTable(inboundCommands, outboundCommands, inboundEvents, outboundEvents);
            Bus = new Kafka.KafkaEventBus("localhost:9092", new JsonSerialization(), routes);
        }

        [Fact]
        public async Task can_send_commands()
        {
            var received = false;
            Bus.MessageReceived += (msg) =>
            {
                Assert.NotNull(msg.Payload);
                received = true;
            };
            await Bus.Send(new CreateUser());
            System.Threading.Thread.Sleep(3000);
            Assert.True(received);
        }

        public void Dispose()
        {
            Bus.Dispose();
        }

        public class CreateUser {}
        public class RegisterUser { }
    }
}
