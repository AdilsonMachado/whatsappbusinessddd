using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhatsAppBusiness.Domain.Entities;
using WhatsAppBusiness.Domain.ValueObjects;

namespace WhatsAppBusiness.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for Conversation entity
/// </summary>
public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.ToTable("Conversations");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .ValueGeneratedNever();

        builder.Property(c => c.UserId)
            .IsRequired();

        builder.Property(c => c.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20)
            .HasConversion(
                phoneNumber => phoneNumber.Value,
                value => PhoneNumber.Create(value));

        builder.Property(c => c.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.UpdatedAt)
            .IsRequired();

        builder.Property(c => c.AssignedOperatorId);

        builder.Property(c => c.IsWithAI)
            .IsRequired();

        builder.HasIndex(c => c.UserId);
        builder.HasIndex(c => c.PhoneNumber);
        builder.HasIndex(c => c.Status);
        builder.HasIndex(c => c.AssignedOperatorId);
    }
}
