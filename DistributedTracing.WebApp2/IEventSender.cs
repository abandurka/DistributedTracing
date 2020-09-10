using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Confluent.Kafka;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;

namespace DistributedTracing.WebApp2
{
    public interface IEventSender
    {
        Task SendAsync(string eventMessage);
    }

    class EventSender : IEventSender
    {
        ProducerConfig _config;

        public EventSender()
        {
            _config = new ProducerConfig
            {
                BootstrapServers = "localhost:9094",
                ClientId = Dns.GetHostName(),
            };
        }

        public IPropagator Propagator { get; set; } = new CompositePropagator(new IPropagator[]
        {
            new TextMapPropagator(),
            new BaggagePropagator(),
        });
        
        public async Task SendAsync(string eventMessage)
        {
            var dict = new Dictionary<string,string>();
            Propagator.Inject(new PropagationContext(Activity.Current.Context, Baggage.Current), dict, (x, k, v) => x[k]=v);

            var traceState = Encoding.Default.GetBytes(JsonSerializer.Serialize(dict));
            using (var producer = new ProducerBuilder<Null, string>(_config).Build())
            {
                var message = new Message<Null, string>()
                {
                    Value = eventMessage,
                    Headers = new Headers()
                    {
                        new Header("trace-context", traceState)
                    }
                };
                
                await producer.ProduceAsync("KF.Events", message);
            }
        }
    }
}