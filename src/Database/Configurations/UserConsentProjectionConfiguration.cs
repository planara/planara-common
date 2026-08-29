using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planara.Common.Database.Domain;

namespace Planara.Common.Database.Configurations;

public class UserConsentProjectionConfiguration : IEntityTypeConfiguration<UserConsentProjection>
{
    public void Configure(EntityTypeBuilder<UserConsentProjection> builder)
    {
        builder.ToTable("UserConsentProjections");

        builder.HasKey(x => new
        {
            x.UserId,
            x.Type
        });

        builder.HasIndex(x => x.ConsentVersionId);
    }
}