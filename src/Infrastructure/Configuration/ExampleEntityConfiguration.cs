using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration
{
    [ExcludeFromCodeCoverage]
    public class ExampleEntityConfiguration : IEntityTypeConfiguration<Example>
    {
        public void Configure(EntityTypeBuilder<Example> builder)
        {
            builder.ToTable("examples");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasColumnName("id")
                .HasMaxLength(40)
                .HasColumnType("varchar(40)")
                .IsRequired();

            builder.Property(e => e.ZipCode)
                .HasColumnName("zip_code")
                .HasMaxLength(10)
                .HasColumnType("varchar(10)")
                .IsRequired();

            builder.Property(e => e.Street)
                .HasColumnName("street")
                .HasMaxLength(255)
                .HasColumnType("varchar(255)");

            builder.Property(e => e.Complement)
                .HasColumnName("complement")
                .HasMaxLength(255)
                .HasColumnType("varchar(255)");

            builder.Property(e => e.Unit)
                .HasColumnName("unit")
                .HasMaxLength(255)
                .HasColumnType("varchar(255)");

            builder.Property(e => e.Neighborhood)
                .HasColumnName("neighborhood")
                .HasMaxLength(100)
                .HasColumnType("varchar(100)");

            builder.Property(e => e.City)
                .HasColumnName("city")
                .HasMaxLength(100)
                .HasColumnType("varchar(100)")
                .IsRequired();

            builder.Property(e => e.State)
                .HasColumnName("state")
                .HasMaxLength(2)
                .HasColumnType("char(2)")
                .IsRequired();

            builder.Property(e => e.CreatedDate)
                .HasColumnName("created_date")
                .HasColumnType("datetime")
                .IsRequired();

            builder.Property(e => e.UpdatedDate)
                .HasColumnName("updated_date")
                .HasColumnType("datetime")
                .IsRequired();

            builder.HasIndex(e => e.ZipCode)
                .HasDatabaseName("idx_zip_codes_zip_code")
                .IsUnique();

            builder.HasIndex(e => new { e.State, e.City })
                .HasDatabaseName("idx_zip_codes_state_city");

            builder.HasIndex(e => e.Street)
                .HasDatabaseName("idx_zip_codes_street");
        }
    }
}
