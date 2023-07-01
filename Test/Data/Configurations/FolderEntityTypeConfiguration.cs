using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Test.Models;

namespace Test.Data.Configurations;

public class FolderEntityTypeConfiguration : IEntityTypeConfiguration<Folder>
{
    public void Configure(EntityTypeBuilder<Folder> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Id);

        builder.Property(x => x.Name).HasMaxLength(256);

        builder.HasMany(x => x.Children)
            .WithOne(x => x.Parent)
            .HasForeignKey(x => x.ParentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}