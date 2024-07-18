using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataEF.Configurations;
public class ClientSupportConfig : IEntityTypeConfiguration<ClientSupport>
{
    public void Configure(EntityTypeBuilder<ClientSupport> builder)
    {
        builder.HasKey(x => new { x.SupportId, x.ClientId });

        builder.HasOne(x => x.Support)
            .WithMany(s => s.ClientSupport)
            .HasForeignKey(x => x.SupportId);

        builder.HasOne(x => x.Client)
            .WithMany(s => s.ClientSupport)
            .HasForeignKey(x => x.ClientId);
    }
}
