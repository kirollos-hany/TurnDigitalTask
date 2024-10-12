using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TurnDigital.Domain.Logging;
using TurnDigital.Domain.Logging.Entities;
using TurnDigital.Domain.Logging.Enums;

namespace TurnDigital.Infrastructure.DataAccess.Configuration;

public class ActionAuditConfiguration : IEntityTypeConfiguration<ActionAudit>
{
    public void Configure(EntityTypeBuilder<ActionAudit> builder)
    {
        builder
            .ToTable("ActionsAudits");

        builder
            .HasOne(audit => audit.User)
            .WithMany()
            .HasForeignKey(audit => audit.UserId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        builder
            .OwnsOne(audit => audit.DeviceInfo);

        builder
            .Property(audit => audit.Type)
            .HasConversion(new EnumToStringConverter<AuditActionType>());
    }
}