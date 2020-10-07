using System;
using System.Collections.Generic;
using System.Text;
using HEAL.Entities.Objects.Dwh.DataVaultV2;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HEAL.Entities.DataAccess.EFCore.Dwh.DataVaultV2 {

  public static class Defaults {

    public const int HashTextLength = 32;

    public const string SchemaName = "dbo";

    public const string LoadDateColumnName = "LoadDate";
    public const string EndDateColumnName = "EndDate";
    public const string ReferenceColumnName = "Reference";

    public const string LoadedByColumnName = "LoadedBy";
    public const int LoadedByTextLength = 255;
    public const string RecordSourceColumnName = "RecordSource";
    public const int RecordSourceTextLength = 255;

    public const string AuditReferenceColumnName = "AuditReference";
  }

  public static class DVEntityMappingExtensions {


    #region AUDIT Configuration

    public static PropertyBuilder<string> ReferencesAuditInformation<TEntity>
                      (this EntityTypeBuilder<TEntity> thiz
                      , string auditReferenceColumnName = Defaults.AuditReferenceColumnName
                      , int hashTextLength = Defaults.HashTextLength)
                where TEntity : class, IReferenceLoadInformation<string> {
      return ReferencesAuditInformation<TEntity, string>(thiz, auditReferenceColumnName).HasMaxLength(hashTextLength);
    }

    public static PropertyBuilder<long> ReferencesAuditInformation<TEntity>
                      (this EntityTypeBuilder<TEntity> thiz
                      , string auditReferenceColumnName = Defaults.AuditReferenceColumnName)
                where TEntity : class, IReferenceLoadInformation<long> {
      return ReferencesAuditInformation<TEntity, long>(thiz, auditReferenceColumnName);
    }

    public static PropertyBuilder<TReferenceType> ReferencesAuditInformation<TEntity,TReferenceType>
                      (this EntityTypeBuilder<TEntity> thiz
                      , string auditReferenceColumnName = Defaults.AuditReferenceColumnName)
                where TEntity : class, IReferenceLoadInformation<TReferenceType> {
      return thiz.Property(hub => hub.LoadReference).HasColumnName(auditReferenceColumnName).IsRequired();
    }

    public static void StoresAuditInformation<TEntity>
                      (this EntityTypeBuilder<TEntity> thiz
                      , string loadedByColumnName = Defaults.LoadedByColumnName
                      , int loadedByTextLength = Defaults.LoadedByTextLength
                      , string recordSourceColumnName = Defaults.RecordSourceColumnName
                      , int recordSourceTextLength = Defaults.RecordSourceTextLength)
                where TEntity : class, IStoreLoadInformation {

      thiz.Property(hub => hub.LoadedBy).IsRequired()
          .HasColumnName(loadedByColumnName).HasMaxLength(loadedByTextLength);
      thiz.Property(hub => hub.RecordSource).IsRequired()
          .HasColumnName(recordSourceColumnName).HasMaxLength(recordSourceTextLength);
    }
    #endregion

    #region HUB Configuration
    public static void ConfigureDataVaultHub<TEntity, THubReference>
                      (this EntityTypeBuilder<TEntity> thiz
                      , string table
                      , string primaryKeyColumn
                      , string schema = Defaults.SchemaName
                      , string loadDateColumnName = Defaults.LoadDateColumnName
                      , int hashTextLength = Defaults.HashTextLength)
                where TEntity : class, IHub<THubReference>
                where THubReference : IComparable<THubReference> {

      thiz.UseDomainObjectTable<TEntity, THubReference>
          (table, schema, primaryKeyColumn);
      thiz.Property(hub => hub.PrimaryKey).HasMaxLength(hashTextLength);
      thiz.Property(b => b.LoadDate).HasColumnName(loadDateColumnName).IsRequired();
    }

    public static void ConfigureDataVaultHub<TEntity>
                      (this EntityTypeBuilder<TEntity> thiz
                      , string table
                      , string primaryKeyColumn
                      , string schema = Defaults.SchemaName
                      , string loadDateColumnName = Defaults.LoadDateColumnName
                      , int hashTextLength = Defaults.HashTextLength)
                where TEntity : class, IHub {
      ConfigureDataVaultHub<TEntity, string>(thiz, table, primaryKeyColumn, schema, loadDateColumnName, hashTextLength);
    }
    #endregion

    #region LINK Configuration
    public static void ConfigureDataVaultLink<TEntity, THubReference>
                      (this EntityTypeBuilder<TEntity> thiz
                      , string table
                      , string primaryKeyColumn
                      , string schema = Defaults.SchemaName
                      , string loadDateColumnName = Defaults.LoadDateColumnName
                      , int hashTextLength = Defaults.HashTextLength)
                where TEntity : class, ILink<THubReference>
                where THubReference : IComparable<THubReference> {
      thiz.UseDomainObjectTable<TEntity, THubReference>
          (table, schema, primaryKeyColumn);
      thiz.Property(hub => hub.PrimaryKey).HasMaxLength(hashTextLength);

      thiz.Property(b => b.LoadDate).HasColumnName(loadDateColumnName).IsRequired();
    }

    public static void ConfigureDataVaultLink<TEntity>
                      (this EntityTypeBuilder<TEntity> thiz
                      , string table
                      , string primaryKeyColumn
                      , string schema = Defaults.SchemaName
                      , string loadDateColumnName = Defaults.LoadDateColumnName
                      , int hashTextLength = Defaults.HashTextLength)
                where TEntity : class, ILink {
      ConfigureDataVaultLink<TEntity, string>(thiz, table, primaryKeyColumn, schema, loadDateColumnName, hashTextLength);
    }

    public static void ConfigureDataVaultTimelineLink<TEntity,TTimelineSattelite>
                      (this EntityTypeBuilder<TEntity> thiz
                      , string table
                      , string primaryKeyColumn
                      , string schema = Defaults.SchemaName
                      , string loadDateColumnName = Defaults.LoadDateColumnName
                      , int hashTextLength = Defaults.HashTextLength)
                where TEntity : class, ILinkTimeline<string,TTimelineSattelite>
                where TTimelineSattelite : ITimelineSatellite<string>{
      ConfigureDataVaultLink<TEntity, string>(thiz, table, primaryKeyColumn, schema, loadDateColumnName, hashTextLength);
    }
    #endregion


    #region SATELLITE Configuration
    public static void ConfigureDataVaultSatellite<TEntity, THubReference>
                  (this EntityTypeBuilder<TEntity> thiz
                  , string table
                  , string schema = Defaults.SchemaName
                  , string loadDateColumnName = Defaults.LoadDateColumnName
                  , int hashTextLength = Defaults.HashTextLength
                  , string referenceColumn = Defaults.ReferenceColumnName)
            where TEntity : class, ISatellite<THubReference>
            where THubReference : IComparable<THubReference> {
      thiz.UseDomainObjectTable<TEntity, ISatellitePrimaryKey<THubReference>>
          (table, schema, s => new { s.Reference, s.LoadDate });

      thiz.Ignore(sat => sat.PrimaryKey);

      thiz.Property(sat => sat.Reference)
          .HasColumnName(referenceColumn)
          .HasMaxLength(hashTextLength)
          .IsRequired();

      thiz.Property(sat => sat.LoadDate).HasColumnName(loadDateColumnName).IsRequired();
    }

    public static void ConfigureDataVaultSatellite<TEntity>
                      (this EntityTypeBuilder<TEntity> thiz
                      , string table
                      , string schema = Defaults.SchemaName
                      , string loadDateColumnName = Defaults.LoadDateColumnName
                      , int hashTextLength = Defaults.HashTextLength
                      , string referenceColumn = Defaults.ReferenceColumnName)
                where TEntity : class, ISatellite {
      ConfigureDataVaultSatellite<TEntity, string>(thiz, table, schema, loadDateColumnName, hashTextLength, referenceColumn);
    }
    #endregion


    #region Deleteable-Link TIMELINE SATELLITE Configuration
    public static void ConfigureDataVaultLinkTimeline<TTimelineEntry, THubReference>
                      (this EntityTypeBuilder<TTimelineEntry> thiz
                      , string table
                      , string schema = Defaults.SchemaName
                      , string loadDateColumnName = Defaults.LoadDateColumnName
                      , string endDateColumnName = Defaults.EndDateColumnName
                      , int hashTextLength = Defaults.HashTextLength
                      , string referenceColumn = Defaults.ReferenceColumnName)
                where TTimelineEntry : class, ITimelineSatellite<THubReference>
                where THubReference : IComparable<THubReference> {

      ConfigureDataVaultSatellite<TTimelineEntry, THubReference>(thiz, table, schema, loadDateColumnName, hashTextLength, referenceColumn);
      thiz.Property(sat => sat.EndDate).HasColumnName(endDateColumnName);
    }

    public static void ConfigureDataVaultLinkTimeline<TTimelineEntry>
                      (this EntityTypeBuilder<TTimelineEntry> thiz
                      , string table
                      , string schema = Defaults.SchemaName
                      , string loadDateColumnName = Defaults.LoadDateColumnName
                      , string endDateColumnName = Defaults.EndDateColumnName
                      , int hashTextLength = Defaults.HashTextLength
                      , string referenceColumn = Defaults.ReferenceColumnName)
                where TTimelineEntry : class, ITimelineSatellite<string> {
      ConfigureDataVaultLinkTimeline<TTimelineEntry, string>(thiz, table, schema, loadDateColumnName, endDateColumnName, hashTextLength, referenceColumn);
    }
    #endregion
  }
}
