using Microsoft.EntityFrameworkCore;
using WhatsAppBusiness.Domain.Entities;

namespace WhatsAppBusiness.Infrastructure.Data;

/// <summary>
/// Entity Framework Core database context for WhatsApp Business application
/// </summary>
public class WhatsAppBusinessDbContext : DbContext
{
    /// <summary>
    /// Gets or sets the Users DbSet
    /// </summary>
    public DbSet<User> Users { get; set; } = null!;

    /// <summary>
    /// Gets or sets the Conversations DbSet
    /// </summary>
    public DbSet<Conversation> Conversations { get; set; } = null!;

    /// <summary>
    /// Gets or sets the Messages DbSet
    /// </summary>
    public DbSet<Message> Messages { get; set; } = null!;

    /// <summary>
    /// Gets or sets the Operators DbSet
    /// </summary>
    public DbSet<Operator> Operators { get; set; } = null!;

    /// <summary>
    /// Gets or sets the WhatsAppConfigurations DbSet
    /// </summary>
    public DbSet<WhatsAppConfiguration> WhatsAppConfigurations { get; set; } = null!;

    public WhatsAppBusinessDbContext(DbContextOptions<WhatsAppBusinessDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from the assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WhatsAppBusinessDbContext).Assembly);
    }
}
