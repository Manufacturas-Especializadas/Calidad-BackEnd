using Microsoft.EntityFrameworkCore;
using Rechazos.Dtos;
using System;
using System.Collections.Generic;

namespace Rechazos.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Clients> Clients { get; set; }

    public virtual DbSet<Lines> Lines { get; set; }

    public virtual DbSet<Rejections> Rejections { get; set; }

    public virtual DbSet<RjCondition> RjCondition { get; set; }

    public virtual DbSet<RjContainmentaction> RjContainmentaction { get; set; }

    public virtual DbSet<RjDefects> RjDefects { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {


        modelBuilder.Entity<Clients>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Clients__3214EC07F5814F07");

            entity.Property(e => e.Name)
                .HasMaxLength(70)
                .IsUnicode(false)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Lines>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Lines__3214EC0738107F81");

            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Rejections>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Rejectio__3214EC0721BA18FD");

            entity.Property(e => e.Description)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.Folio)
                .HasDefaultValueSql("(NEXT VALUE FOR [Seq_Folio])")
                .HasColumnName("folio");
            entity.Property(e => e.Image)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("image");
            entity.Property(e => e.InformedSignature)
                .HasMaxLength(300)
                .IsUnicode(false)
                .HasColumnName("informedSignature");
            entity.Property(e => e.Insepector)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("insepector");
            entity.Property(e => e.NumberOfPieces).HasColumnName("numberOfPieces");
            entity.Property(e => e.OperatorPayroll).HasColumnName("operatorPayroll");
            entity.Property(e => e.PartNumber)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("partNumber");
            entity.Property(e => e.RegistrationDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("registrationDate");

            entity.HasOne(d => d.IdClientNavigation).WithMany(p => p.Rejections)
                .HasForeignKey(d => d.IdClient)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Rejection__IdCli__17036CC0");

            entity.HasOne(d => d.IdConditionNavigation).WithMany(p => p.Rejections)
                .HasForeignKey(d => d.IdCondition)
                .HasConstraintName("FK__Rejection__IdCon__151B244E");

            entity.HasOne(d => d.IdContainmentactionNavigation).WithMany(p => p.Rejections)
                .HasForeignKey(d => d.IdContainmentaction)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Rejection__IdCon__17F790F9");

            entity.HasOne(d => d.IdDefectNavigation).WithMany(p => p.Rejections)
                .HasForeignKey(d => d.IdDefect)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Rejection__IdDef__14270015");

            entity.HasOne(d => d.IdLineNavigation).WithMany(p => p.Rejections)
                .HasForeignKey(d => d.IdLine)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Rejection__IdLin__160F4887");
        });

        modelBuilder.Entity<RjCondition>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RjCondit__3214EC076CC445F3");

            entity.Property(e => e.Name)
                .HasMaxLength(70)
                .IsUnicode(false)
                .HasColumnName("name");

            entity.HasOne(d => d.IdDefectsNavigation).WithMany(p => p.RjCondition)
                .HasForeignKey(d => d.IdDefects)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__RjConditi__IdDef__74AE54BC");
        });

        modelBuilder.Entity<RjContainmentaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RjContai__3214EC072DC21D99");

            entity.Property(e => e.Name)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("name");
        });

        modelBuilder.Entity<RjDefects>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RjDefect__3214EC070A77E270");

            entity.Property(e => e.Name)
                .HasMaxLength(70)
                .IsUnicode(false)
                .HasColumnName("name");
        });
        modelBuilder.HasSequence("Seq_Folio");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}