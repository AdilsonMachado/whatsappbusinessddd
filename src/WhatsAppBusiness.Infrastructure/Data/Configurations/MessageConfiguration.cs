using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhatsAppBusiness.Domain.Entities;
using WhatsAppBusiness.Domain.ValueObjects;

namespace WhatsAppBusiness.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for Message entity
/// </summary>
public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("Messages");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id)
            .ValueGeneratedNever();

        builder.Property(m => m.ConversationId)
            .IsRequired();

        builder.Property(m => m.Content)
            .IsRequired()
            .HasMaxLength(4096)
            .HasConversion(
                content => content.Value,
                value => MessageContent.Create(value));

        builder.Property(m => m.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(m => m.Direction)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(m => m.SentAt)
            .IsRequired();

        builder.Property(m => m.DeliveredAt);

        builder.Property(m => m.ReadAt);

        builder.Property(m => m.IsFromAI)
            .IsRequired();

        builder.Property(m => m.WhatsAppMessageId)
            .HasMaxLength(100);

        builder.HasIndex(m => m.ConversationId);
        builder.HasIndex(m => m.WhatsAppMessageId);
        builder.HasIndex(m => m.SentAt);
    }
}
