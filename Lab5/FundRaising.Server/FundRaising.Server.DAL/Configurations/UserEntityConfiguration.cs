using FundRaising.Server.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FundRaising.Server.DAL.Configurations;

public class UserEntityConfiguration
    : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasColumnName("id");
        
        builder.Property(p => p.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        
        builder.Property(p => p.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
        
        builder.Property(p => p.Id)
            .HasColumnName("id");
        
        builder.Property(p => p.Email)
            .HasColumnType("varchar(256)")
            .HasColumnName("email")
            .IsRequired();
        
        builder.HasIndex(p => p.Email).IsUnique();

        builder.Property(p => p.PasswordHash)
            .IsRequired()
            .HasMaxLength(512)
            .HasColumnType("varchar(512)")
            .HasColumnName("password_hash");
    }
}