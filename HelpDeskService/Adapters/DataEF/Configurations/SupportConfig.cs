using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataEF.Configurations;
public class SupportConfig : IEntityTypeConfiguration<Support>
{
    public void Configure(EntityTypeBuilder<Support> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.HasIndex(x => x.Email).IsUnique();

        builder.Property(x => x.PasswordHash)
            .HasColumnType("TEXT")
            .IsRequired();

        builder.HasMany(x => x.ClientSupport)
            .WithOne(t => t.Support)
            .HasForeignKey(t => t.SupportId);

        builder.HasMany(x => x.Tickets)
            .WithOne(t => t.Support)
            .HasForeignKey(t => t.SupportId);
    }
}
