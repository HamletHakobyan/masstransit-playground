using MassTransit;

namespace Playground;

// [ExcludeFromTopology]
public interface PrioritizedMessage
{
    public string Name { get; }
}
