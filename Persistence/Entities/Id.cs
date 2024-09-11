namespace Playground.Persistence.Entities;

public sealed record Id<T>(Guid Value) : IComparable,
    IComparable<Id<T>>,
    IComparable<Guid>,
    IEquatable<Id<T>>
    where T : Entity<T>
{
    public Id()
        : this(Guid.NewGuid())
    {
    }
    
    public static Id<T> New() => new(Guid.NewGuid());
    
    public static Id<T> From(Guid id) => new(id);
    
    public static implicit operator Guid?(Id<T>? id) => id?.Value;
    public static implicit operator Guid(Id<T> id) => id.Value;
    public static implicit operator Id<T>(Guid id) => new(id);

    public Guid Value { get; private init; } = Value;

    public int CompareTo(object? obj) => obj switch
    {
        null => 1,
        Id<T> id => CompareTo(id),
        Guid id => CompareTo(id),
        _ => throw new ArgumentException($"Argument must be of type {nameof(Id<T>)}"),
    };
    public int CompareTo(Id<T>? other) => other?.Value.CompareTo(Value) ?? 1;
    public int CompareTo(Guid other) => other.CompareTo(Value);
}

public static class IdEx
{
    public static Id<T> ToEntityId<T>(this Guid id) 
        where T : Entity<T> => new(id);

    public static Guid ToGuid<T>(this Id<T> id)
        where T : Entity<T> => id.Value;
}