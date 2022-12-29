using System;
using System.Collections.Generic;
using Entities.Domain;
using Microsoft.EntityFrameworkCore;

namespace Entities.DataContext;

public partial class AzureServiceBusContext : DbContext
{
    public AzureServiceBusContext()
    {
    }

    public AzureServiceBusContext(DbContextOptions<AzureServiceBusContext> options)
        : base(options)
    {
    }

    
    public virtual DbSet<Vuelo> Vuelos { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Here is your connection string");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        

        modelBuilder.Entity<Vuelo>(entity =>
        {
            entity.ToTable("vuelos", tb => tb.HasTrigger("trRegistrosVuelos_Insert"));

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CantidadEscalas).HasColumnName("cantidad_escalas");
            entity.Property(e => e.CantidadPasajeros).HasColumnName("cantidad_pasajeros");
            entity.Property(e => e.FechaVuelo)
                .HasColumnType("datetime")
                .HasColumnName("fecha_vuelo");
            entity.Property(e => e.NumeroVuelo)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("numero_vuelo");
            entity.Property(e => e.Registrado).HasColumnName("registrado");
            entity.Property(e => e.UbicacionDestino)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("ubicacion_destino");
            entity.Property(e => e.UbicacionOrigen)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("ubicacion_origen");
        });

        
        

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
