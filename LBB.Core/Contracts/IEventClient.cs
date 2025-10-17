namespace LBB.Core.Contracts;

public interface IEventClient
{
    Task EventReceived(string topic, object payload);
}
