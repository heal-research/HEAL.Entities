using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using HEAL.Entities.DataAccess.Caching.Abstractions;
using HEAL.Entities.DataAccess.Dwh.DataVaultV2.Generic.Abstractions;
using HEAL.Entities.DataAccess.EFCore.Caching;
using HEAL.Entities.Objects.Dwh.DataVaultV2;
using HEAL.Entities.Utils.Enumerables;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HEAL.Entities.DataAccess.EFCore.Dwh.DataVaultV2.Generic {
  /// <summary>
  /// derive from this class to inherit the base implementation for repository data-access to a DWH in DataVault 2.0 Schema <br></br>
  /// inherits from <see cref="LinkRepository{TEntity, TPrimaryKey}"/> and extends functionality for 
  /// </summary>
  /// <typeparam name="TLink">Generic type of the managed domain entity</typeparam>
  /// <typeparam name="TPrimaryKey">type of the primary key</typeparam>
  /// <typeparam name="TSatelliteTimeline">type of the timeline entity</typeparam>
  public abstract class LinkTimelineRepository<TLink, TPrimaryKey, TSatelliteTimeline, TReference> : LinkRepository<TLink, TPrimaryKey, TReference>
                                                                    , ILinkTimelineRepository<TLink, TPrimaryKey, TSatelliteTimeline, TReference>
      where TLink : class, ILinkTimeline<TPrimaryKey, TSatelliteTimeline>
      where TSatelliteTimeline : class, ITimelineSatellite<TPrimaryKey>, new()
      where TPrimaryKey : IComparable<TPrimaryKey> {

    /// <summary>
    /// internal DbSet of the generic EFDataVaultRepository
    /// Use this property for manual Queries needed in the specific DomainObject
    /// </summary>
    protected DbSet<TSatelliteTimeline> DbSetTimeline;


    /// <summary>
    /// Creates a new <see cref="LinkRepository{TEntity, TPrimaryKey, TReference}"/>
    /// </summary>
    /// <param name="context"><see cref="DwhDbContext"/> apply performance improvements per default</param>
    /// <param name="hashFunction">any hash function reducing the hubs business-key to it's primary key</param>
    /// <param name="logger"></param>
    public LinkTimelineRepository(DwhDbContext context
                                    , DataVaultHashFunction<TPrimaryKey> hashFunction
                                  , IPrimaryKeyCache keyCache = null
                                  , DVKeyCaching useKeyCaching = DVKeyCaching.Disabled
                                  , ILogger<LinkTimelineRepository<TLink, TPrimaryKey, TSatelliteTimeline, TReference>> logger = null)
                              : base(context, hashFunction, keyCache,useKeyCaching,logger) {
      DbSetTimeline = Context.Set<TSatelliteTimeline>();
    }

    protected override TLink AddEntity(TLink entity, DateTime loadDate
                                  , IStoreLoadInformation loadInformation
                                  , IReferenceLoadInformation<TReference> loadReference
                                  , IEditedByInformation editedByInformation) {
      entity = base.AddEntity(entity, loadDate, loadInformation, loadReference, editedByInformation);
      AddTimeLineEntry(entity, loadDate, loadInformation, loadReference, editedByInformation);
      return entity;
    }

    private void AddTimeLineEntry(TLink entity, DateTime loadDate
                                  , IStoreLoadInformation loadInformation
                                  , IReferenceLoadInformation<TReference> loadReference
                                  , IEditedByInformation editedByInformation) {
      //timeline has no active entries
      TSatelliteTimeline defaultEntry = CreateDefaulTSatelliteTimeline(entity);

      SetLoadReference(defaultEntry as IReferenceLoadInformation<TReference>, loadReference);
      SetStoreLoadInformation(defaultEntry as IStoreLoadInformation, loadInformation);
      SetEditedBy(defaultEntry as IEditedByInformation, editedByInformation);

      if (UseKeyCaching == DVKeyCaching.Enabled) {

        //we insert link timeline entries only if there is no active one
        // the timeline cache only keeps active keys
        if (AddIfKeyIsUnique<TSatelliteTimeline>(entity.PrimaryKey)) {
          DbSetTimeline.Add(defaultEntry);
        } else {
          Logger?.LogTrace($"Primary key '{entity.PrimaryKey}' was already contained in '{nameof(IPrimaryKeyCache)}' for type '{typeof(TSatelliteTimeline)}'. Timeline is up to date.");
        }
      } else {
        TSatelliteTimeline current = GetCurrentOrDefaulTSatelliteTimeline(entity.PrimaryKey);

        if (current == default || current.EndDate != null) {
          DbSetTimeline.Add(defaultEntry);
        }
      }
    }

    protected virtual TSatelliteTimeline CreateDefaulTSatelliteTimeline(TLink entity) {
      var defaultEntry = new TSatelliteTimeline();
      defaultEntry.LoadDate = entity.LoadDate;
      defaultEntry.Reference = entity.PrimaryKey;
      return defaultEntry;
    }

    protected bool RemoveIfKeyIsUnique<T>(TPrimaryKey primaryKey) {
      var PrimaryKeys = KeyCache.GetCachedKeys<T, TPrimaryKey>(typeof(T));
      return PrimaryKeys.Remove(primaryKey);
    }

    public override IEnumerable<TLink> GetCurrent(
        Expression<Func<TLink, bool>> filter = null,
        Func<IQueryable<TLink>, IOrderedQueryable<TLink>> orderBy = null,
        string includeProperties = "") {
      IQueryable<TLink> query = DbSet;

      query.Include(x => x.Timeline);
      query.Where(x => x.MostRecentEntry.EndDate == null);

      if (filter != null) {
        query = query.Where(filter);
      }

      foreach (var includeProperty in includeProperties.Split
          (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)) {
        query = query.Include(includeProperty);
      }

      if (orderBy != null) {
        return orderBy(query).ToList();
      } else {
        return query.ToList();
      }
    }

    protected virtual TSatelliteTimeline GetCurrentOrDefaulTSatelliteTimeline(TPrimaryKey primaryKey) {
      return DbSetTimeline.AsTracking().Where(x => x.Reference.CompareTo(primaryKey) == 0).MaxOrDefaultBy(x => x.LoadDate);
    }

    protected virtual void RemoveOneLink(TLink entity, DateTime removalDate) {
      if (UseKeyCaching == DVKeyCaching.Enabled) {

        //we insert link timeline entries only if there is no active one
        // the timeline cache only keeps active keys
        if (RemoveIfKeyIsUnique<TSatelliteTimeline>(entity.PrimaryKey)) {
          TSatelliteTimeline current = GetCurrentOrDefaulTSatelliteTimeline(entity.PrimaryKey);
          SetAndTrackRemoval(removalDate, current);
        } else {
          Logger?.LogTrace($"Primary key '{entity.PrimaryKey}' was already removed from '{nameof(IPrimaryKeyCache)}' for type '{typeof(TSatelliteTimeline)}'. Timeline is up to date.");
        }
      } else {
        TSatelliteTimeline current = GetCurrentOrDefaulTSatelliteTimeline(entity.PrimaryKey);

        if (current == default) {
          return; //entry never existed, must not be deleted
        }
        if (current.EndDate != null) {
          SetAndTrackRemoval(removalDate, current);
        }
      }
    }

    private void SetAndTrackRemoval(DateTime removalDate, TSatelliteTimeline current) {
      current.EndDate = removalDate;
    }

    public void RemoveLink(TLink entity, DateTime removalDate) {
      RemoveOneLink(entity, removalDate);
      Context.ChangeTracker.DetectChanges();
      Context.SaveChanges();
    }

    public void RemoveLink(IEnumerable<TLink> entities, DateTime removalDate) {
      foreach (var entity in entities) {
        RemoveOneLink(entity, removalDate);
      }
      Context.ChangeTracker.DetectChanges();
      Context.SaveChanges();
    }


    protected override void InitializePrimaryKeyCache() {
      var entries = DbSet.Include(x => x.Timeline).ToList();

      var allPrimaryKeys = entries.Select(x => x.PrimaryKey).Distinct();
      var activePrimaryKeys = entries.Where(x => x.MostRecentEntry?.EndDate == null).Select(x => x.PrimaryKey).Distinct();

      var cache = new HashSet<TPrimaryKey>(allPrimaryKeys);
      Logger?.LogTrace($"Initializing primary key link hash cache. [{cache.Count}] entries found in database.");
      KeyCache.InitializeKeyCache<TLink, TPrimaryKey>(typeof(TLink), cache);

      cache = new HashSet<TPrimaryKey>(activePrimaryKeys);
      Logger?.LogTrace($"Initializing primary key active link hash cache. [{cache.Count}] entries found in database.");
      KeyCache.InitializeKeyCache<TSatelliteTimeline, TPrimaryKey>(typeof(TSatelliteTimeline), cache);
    }
  }

}