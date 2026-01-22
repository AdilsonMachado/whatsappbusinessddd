using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhatsAppBusiness.Domain.Entities;

namespace WhatsAppBusiness.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for WhatsAppConfiguration entity
/// </summary>
public class WhatsAppConfigurationConfiguration : IEntityTypeConfiguration<WhatsAppConfiguration>
{
    public void Configure(EntityTypeBuilder<WhatsAppConfiguration> builder)
    {
        builder.ToTable("WhatsAppConfigurations");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .ValueGeneratedNever();

        builder.Property(c => c.PhoneNumberId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.AccessToken)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(c => c.WebhookVerifyToken)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(c => c.BusinessAccountId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.UpdatedAt)
            .IsRequired();
    }
}
