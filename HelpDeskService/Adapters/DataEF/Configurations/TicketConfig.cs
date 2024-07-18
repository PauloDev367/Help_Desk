using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataEF.Configurations;
public class TicketConfig : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.HasOne(x => x.Client)
            .WithMany(c => c.Tickets)
            .IsRequired(true);

        builder.HasOne(x => x.Support)
            .WithMany(s => s.Tickets)
            .IsRequired(false);
    }
}
