using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Model;

public partial class DbMarxWeatherStationContext : DbContext
{
    public DbMarxWeatherStationContext()
    {
    }

    public DbMarxWeatherStationContext(DbContextOptions<DbMarxWeatherStationContext> options)
        : base(options)
    {
    }

    public virtual DbSet<WeatherData> WeatherData { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=Default");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WeatherData>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__WeatherD__3214EC073F51F208");

            entity.Property(e => e.Timestamp).HasDefaultValueSql("(getutcdate())");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
