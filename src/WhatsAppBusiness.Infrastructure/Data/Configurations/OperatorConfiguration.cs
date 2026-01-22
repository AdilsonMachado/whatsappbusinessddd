using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhatsAppBusiness.Domain.Entities;

namespace WhatsAppBusiness.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for Operator entity
/// </summary>
public class OperatorConfiguration : IEntityTypeConfiguration<Operator>
{
    public void Configure(EntityTypeBuilder<Operator> builder)
    {
        builder.ToTable("Operators");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Id)
            .ValueGeneratedNever();

        builder.Property(o => o.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(o => o.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.HasIndex(o => o.Email)
            .IsUnique();

        builder.Property(o => o.PasswordHash)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(o => o.IsOnline)
            .IsRequired();

        builder.Property(o => o.CreatedAt)
            .IsRequired();

        builder.Property(o => o.CurrentConversations)
            .IsRequired();
    }
}
