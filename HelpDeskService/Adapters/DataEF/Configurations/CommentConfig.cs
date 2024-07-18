using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataEF.Configurations;
public class CommentConfig : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(x => x.Text)
            .HasColumnType("text")
            .IsRequired();

        builder.HasOne(x => x.Client)
            .WithMany(c => c.Comments)
            .IsRequired(false);

        builder.HasOne(x => x.Support)
            .WithMany(c => c.Comments)
            .IsRequired(false);
    }
}
