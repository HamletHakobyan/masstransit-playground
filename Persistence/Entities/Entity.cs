namespace Playground.Persistence.Entities;

public abstract class Entity<T>(Id<T> id)
    :  CSharpFunctionalExtensions.Entity<Id<T>>(id)
    where T : Entity<T> 
{
    public override Id<T> Id { get; protected set; } = new();

    public uint Version { get; set; }
}