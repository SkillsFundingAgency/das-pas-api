using Microsoft.EntityFrameworkCore;
using SFA.DAS.PAS.Domain.Entities;

namespace SFA.DAS.PAS.Data;

public interface IPasDataContext
{
    DbSet<Provider> Providers { get; }
    DbSet<User> Users { get; }
    DbSet<UserSettings> UserSettings { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}

public class PasDataContext : DbContext, IPasDataContext
{
    public DbSet<Provider> Providers => Set<Provider>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UserSettings> UserSettings => Set<UserSettings>();
    public PasDataContext() { }
    public PasDataContext(DbContextOptions<PasDataContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PasDataContext).Assembly);
    }
}
