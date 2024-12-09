using FundRaising.Server.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FundRaising.Server.DAL.Configurations;

public class FundraiserEntityConfiguration
    : IEntityTypeConfiguration<Fundraiser>
{
    public void Configure(EntityTypeBuilder<Fundraiser> builder)
    {
        builder.ToTable("fundraisers");
        
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

        builder.Property(p => p.Title)
            .IsRequired()
            .HasColumnType("varchar(200)")
            .HasMaxLength(200)
            .HasColumnName("title");
        
        builder.Property(p => p.Description)
            .IsRequired()
            .HasColumnType("varchar(500)")
            .HasMaxLength(500)
            .HasColumnName("description");
        
        builder.Property(p => p.Goal)
            .IsRequired()
            .HasColumnName("goal");

        builder.Property(p => p.AmountRaised)
            .HasDefaultValue(0)
            .ValueGeneratedOnAdd()
            .HasColumnName("amount_raised");
        
        builder.Property(p => p.UserId)
            .IsRequired()
            .HasColumnName("user_id");
        
        builder.HasOne(f => f.User)
            .WithMany(u => u.Fundraisers)
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}