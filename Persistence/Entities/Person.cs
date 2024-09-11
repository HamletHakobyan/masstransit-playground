namespace Playground.Persistence.Entities;

public class Person : AggregateRoot<Person>
{
    public string Name { get; init; }
    public string Surname { get; init; }
    public string Email { get; init; }
    public int Age { get; init; }
}