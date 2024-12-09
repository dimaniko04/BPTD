using FundRaising.Server.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace FundRaising.Server.DAL.Configurations;

public class DonationEntityConfiguration: IEntityTypeConfiguration<Donation>
{
    public void Configure(EntityTypeBuilder<Donation> builder)
    {
        builder.ToTable("donations");
        
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");
        
        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        
        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
        
        builder.Property(x => x.Id)
            .HasColumnName("id");
        
        builder.Property(x => x.Description)
            .HasColumnType("varchar(200)")
            .HasMaxLength(200)
            .HasColumnName("description");
        
        builder.Property(x => x.Amount)
            .HasColumnName("amount")
            .IsRequired();

        builder.Property(p=> p.UserId)
            .HasColumnName("user_id")
            .IsRequired();
        
        builder.Property(p=> p.FundraiserId)
            .HasColumnName("fundraiser_id")
            .IsRequired();
        
        builder.HasOne(d => d.Fundraiser)
            .WithMany(f => f.Donations)
            .HasForeignKey(d => d.FundraiserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(d => d.User)
            .WithMany(u => u.Donations)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}