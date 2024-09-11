namespace Playground.Persistence.Entities;

public abstract class AggregateRoot<T>() : Entity<T>(Id<T>.New()) where T : Entity<T>
{
}