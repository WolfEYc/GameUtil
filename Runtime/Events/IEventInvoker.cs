namespace Wolfey.Events
{
    public interface IEventInvoker
    {
        public void Invoke();

        public void InvokePayload(object payload);
    }
}
