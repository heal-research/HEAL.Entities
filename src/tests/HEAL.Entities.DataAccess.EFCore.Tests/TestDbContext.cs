using System;
using System.Collections.Generic;
using System.Text;
using HEAL.Entities.DataAccess.EFCore.Dwh;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HEAL.Entities.DataAccess.EFCore.Dwh.DataVaultV2;
using Microsoft.EntityFrameworkCore.Storage;

namespace HEAL.Entities.DataAccess.EFCore.Tests {


  public class TestDwhDbContext : DwhDbContext {
    private const string IdColumnName = "id";

    public static TestDwhDbContext CreateDWHDbContext() {
      var optionsBuilder = new DbContextOptionsBuilder<TestDwhDbContext>();
      optionsBuilder.UseInMemoryDatabase("TestDB", new InMemoryDatabaseRoot())
        .UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll)
        .EnableSensitiveDataLogging();
      return new TestDwhDbContext(optionsBuilder.Options, 300);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="options">the options for this context</param>
    /// <param name="commandExecutionTimeOut">sets timeout for command execution. in SECONDS</param>
    public TestDwhDbContext(DbContextOptions<TestDwhDbContext> options, int commandExecutionTimeOut) : base(options, commandExecutionTimeOut) {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
      base.OnModelCreating(modelBuilder);
      modelBuilder.Entity<H_TestHub_Default>(Configure);
      modelBuilder.Entity<S_TestSatellite_Default>(Configure);
      modelBuilder.Entity<L_TestLink_Default>(Configure);

      modelBuilder.Entity<H_TestHub_Stores>(Configure);
      modelBuilder.Entity<S_TestSatellite_Stores>(Configure);
      modelBuilder.Entity<L_TestLink_Stores>(Configure);

      modelBuilder.Entity<H_TestHub_References>(Configure);
      modelBuilder.Entity<S_TestSatellite_References>(Configure);
      modelBuilder.Entity<L_TestLink_References>(Configure);

      modelBuilder.Entity<LoadInformation>(Configure);

      modelBuilder.Entity<L_Link_Timeline>(Configure);
      modelBuilder.Entity<S_LinkTimelineEntry>(Configure);

    }

    private void Configure(EntityTypeBuilder<LoadInformation> entityBuilder) {
      entityBuilder.UseDomainObjectTable<LoadInformation, long>(nameof(LoadInformation), IdColumnName);
      entityBuilder.StoresAuditInformation();
    }


    private void Configure(EntityTypeBuilder<H_TestHub_Default> entityBuilder) {
      entityBuilder.ConfigureDataVaultHub(nameof(H_TestHub_Default), IdColumnName);
      entityBuilder.Property(b => b.TestNr).IsRequired().HasMaxLength(30);
    }

    private void Configure(EntityTypeBuilder<S_TestSatellite_Default> entityBuilder) {
      entityBuilder.ConfigureDataVaultSatellite(nameof(S_TestSatellite_Default), IdColumnName);
      entityBuilder.Property(b => b.TestNr).IsRequired().HasMaxLength(30);
      entityBuilder.Property(b => b.Value).IsRequired();
    }

    private void Configure(EntityTypeBuilder<L_TestLink_Default> entityBuilder) {
      entityBuilder.ConfigureDataVaultLink(nameof(L_TestLink_Default), IdColumnName);
      entityBuilder.Property(b => b.FirstTest).IsRequired().HasMaxLength(Defaults.HashTextLength);
      entityBuilder.Property(b => b.SecondTest).IsRequired().HasMaxLength(Defaults.HashTextLength);
    }




    private void Configure(EntityTypeBuilder<H_TestHub_References> entityBuilder) {
      entityBuilder.ConfigureDataVaultHub(nameof(H_TestHub_References), IdColumnName);
      entityBuilder.Property(b => b.TestNr).IsRequired().HasMaxLength(30);
      entityBuilder.ReferencesAuditInformation();
    }

    private void Configure(EntityTypeBuilder<S_TestSatellite_References> entityBuilder) {
      entityBuilder.ConfigureDataVaultSatellite(nameof(S_TestSatellite_References), IdColumnName);
      entityBuilder.Property(b => b.TestNr).IsRequired().HasMaxLength(30);
      entityBuilder.Property(b => b.Value).IsRequired();
      entityBuilder.ReferencesAuditInformation();
    }

    private void Configure(EntityTypeBuilder<L_TestLink_References> entityBuilder) {
      entityBuilder.ConfigureDataVaultLink(nameof(L_TestLink_References), IdColumnName);
      entityBuilder.Property(b => b.FirstTest).IsRequired().HasMaxLength(Defaults.HashTextLength);
      entityBuilder.Property(b => b.SecondTest).IsRequired().HasMaxLength(Defaults.HashTextLength);
      entityBuilder.ReferencesAuditInformation();
    }




    private void Configure(EntityTypeBuilder<H_TestHub_Stores> entityBuilder) {
      entityBuilder.ConfigureDataVaultHub(nameof(H_TestHub_Stores), IdColumnName);
      entityBuilder.Property(b => b.TestNr).IsRequired().HasMaxLength(30);
      entityBuilder.StoresAuditInformation();
    }

    private void Configure(EntityTypeBuilder<S_TestSatellite_Stores> entityBuilder) {
      entityBuilder.ConfigureDataVaultSatellite(nameof(S_TestSatellite_Stores), IdColumnName);
      entityBuilder.Property(b => b.TestNr).IsRequired().HasMaxLength(30);
      entityBuilder.Property(b => b.Value).IsRequired();
      entityBuilder.StoresAuditInformation();
    }

    private void Configure(EntityTypeBuilder<L_TestLink_Stores> entityBuilder) {
      entityBuilder.ConfigureDataVaultLink(nameof(L_TestLink_Stores), IdColumnName);
      entityBuilder.Property(b => b.FirstTest).IsRequired().HasMaxLength(Defaults.HashTextLength);
      entityBuilder.Property(b => b.SecondTest).IsRequired().HasMaxLength(Defaults.HashTextLength);
      entityBuilder.StoresAuditInformation();
    }

    private void Configure(EntityTypeBuilder<L_Link_Timeline> entityBuilder) {
      entityBuilder.ConfigureDataVaultTimelineLink<L_Link_Timeline, S_LinkTimelineEntry>(nameof(L_Link_Timeline), IdColumnName);
    }
    private void Configure(EntityTypeBuilder<S_LinkTimelineEntry> entityBuilder) {
      entityBuilder.ConfigureDataVaultLinkTimeline(nameof(S_LinkTimelineEntry), IdColumnName);
    }

  }
}
