using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Hosting.Consuming;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using OpenTelemetry.Trace;


namespace DistributedTracing.WebApp3
{
    class EventConsumer: IHostedConsumer<Null, string>
    {
        private readonly ActivitySourceAdapter _activitySource;

        public EventConsumer(ActivitySourceAdapter activitySource)
        {
            this._activitySource = activitySource;
        }

        public IPropagator Propagator { get; set; } = new CompositePropagator(new IPropagator[]
        {
            new TextMapPropagator(),
            new BaggagePropagator(),
        });

        public async Task ConsumeAsync(Message<Null, string> message, CancellationToken cancellationToken)
        {
            try
            {
                message.Headers.TryGetLastBytes("trace-context", out var content);
                var stringContent = Encoding.UTF8.GetString(content);
                var spanContext = JsonSerializer.Deserialize<Dictionary<string, string>>(stringContent);
                var ctx = Propagator.Extract(default, spanContext, (dictionary, s) => new[] {dictionary[s]});

                Activity newOne = new Activity("kafka-consumer");
                newOne.SetParentId(ctx.ActivityContext.TraceId, ctx.ActivityContext.SpanId, ctx.ActivityContext.TraceFlags);
                newOne.TraceStateString = ctx.ActivityContext.TraceState;
                // Starting the new activity make it the Activity.Current one.
                newOne.Start();
                _activitySource.Start(newOne, ActivityKind.Consumer);
                
                if (ctx.Baggage != default)
                {
                    Baggage.Current = ctx.Baggage;
                }
            
                Console.WriteLine($"Consumed message: {new {message.Key, message.Value}}");
                await Task.Delay(10, cancellationToken);
                newOne.Stop();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}

