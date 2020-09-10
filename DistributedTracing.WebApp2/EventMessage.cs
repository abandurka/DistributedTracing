namespace DistributedTracing.WebApp2
{
    public class EventMessage<T>
    {
        public EventMessage(T content)
        {
            Content = content;
        }

        public T Content { get; private set; }
    }
}