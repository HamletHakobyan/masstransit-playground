using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Playground.Persistence.Entities;

namespace Playground.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Person> Persons { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Person>()
            .Property(p => p.Id)
            .IsRequired()
            .HasConversion(id => id.Value, val => Id<Person>.From(val));;

        builder.Entity<Person>()
            .HasKey(p => p.Id);

        builder.Entity<Person>()
            .Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(128);

        builder.Entity<Person>()
            .Property(p => p.Surname)
            .IsRequired()
            .HasMaxLength(128);

        builder.Entity<Person>()
            .Property(p => p.Email)
            .HasMaxLength(280);

        builder.Entity<Person>()
            .Property<DateTimeOffset>("CreatedAtUtc")
            .IsRequired();
        
        builder.Entity<Person>()
            .Property<DateTimeOffset>("UpdatedAtUtc")
            .IsRequired();
        
    }

    private bool IsGenericType([NotNullWhen(true)]Type? type) =>
        (type?.IsGenericType).GetValueOrDefault();

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entityEntries = ChangeTracker.Entries()
            .Where(entry =>
            {
                var clrType = entry.Metadata.ClrType.BaseType;
                return IsGenericType(clrType) &&
                       clrType.GetGenericTypeDefinition() == typeof(AggregateRoot<>);
            });

        foreach (var entityEntry in entityEntries)
        {
            switch (entityEntry.State)
            {
                case EntityState.Added:
                    entityEntry.Property("CreatedAtUtc").CurrentValue = DateTimeOffset.UtcNow;
                    entityEntry.Property("UpdatedAtUtc").CurrentValue = DateTimeOffset.UtcNow;
                    break;
                case EntityState.Modified:
                    entityEntry.Property("UpdatedAtUtc").CurrentValue = DateTimeOffset.UtcNow;
                    break;                    
            }
        }
        
        return base.SaveChangesAsync(cancellationToken);
    }
}