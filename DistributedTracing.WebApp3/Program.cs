using Confluent.Kafka;
using Confluent.Kafka.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace DistributedTracing.WebApp3
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
                .UseConsumer<EventConsumer, Null, string>((_ , o)=>
                {
                    o.AddConfiguration(new ConsumerConfig
                    {
                        BootstrapServers = "localhost:9094",
                        GroupId = "test-group",
                    });
                    o.AddTopics(new []{"KF.Events"});
                });
    }
}