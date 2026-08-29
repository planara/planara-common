using Microsoft.EntityFrameworkCore;
using Planara.Common.Database.Configurations;

namespace Planara.Common.Database;

public static class ModelBuilderExtensions
{
    public static ModelBuilder AddOutbox(this ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());

        return modelBuilder;
    }

    public static ModelBuilder AddConsentProjections(this ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConsentProjectionConfiguration());

        return modelBuilder;
    }

    public static ModelBuilder AddCommonConfigurations(this ModelBuilder modelBuilder)
    {
        return modelBuilder
            .AddOutbox()
            .AddConsentProjections();
    }
}