using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using HEAL.Entities.DataAccess.Dwh.DataVaultV2.Generic.Abstractions;
using HEAL.Entities.Objects.Dwh.DataVaultV2;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HEAL.Entities.DataAccess.EFCore.Dwh.DataVaultV2.Generic {
  /// <summary>
  /// derive from this class to inherit the base implementation for repository data-access to a DWH in DataVault 2.0 Schema
  /// inserts a new satellite instance (row) for each import run
  /// </summary>
  /// <typeparam name="TEntity">Generic type of the managed domain entity</typeparam>
  /// <typeparam name="TPKey">type of the primary key</typeparam>
  public class SatelliteRepository<TEntity, THubReference, TReference> : BaseRepository<TEntity, ISatellitePrimaryKey<THubReference>, THubReference,TReference>
                                                                      , ISatelliteRepository<TEntity, THubReference, TReference>
      where TEntity : class, ISatellite<THubReference>
      where THubReference : IComparable<THubReference> {

    /// <summary>
    /// Creates a new <see cref="HubRepository{TEntity, THubReference}"/>
    /// </summary>
    /// <param name="context"><see cref="DwhDbContext"/> apply performance improvements per default</param>
    /// <param name="hashFunction">any hash function reducing the hubs business-key to it's primary key</param>
    /// <param name="logger"></param>
    /// <param name="useKeyCaching">Enables or Disables (Default) the caching if primary keys -> entrie does not get added to db set if the key was seen before. Cache is built up on creation of the repo. </param>
    public SatelliteRepository(DwhDbContext context
                                          , DataVaultHashFunction<THubReference> hubKeyHashFunction
                                          , ILogger<SatelliteRepository<TEntity, THubReference, TReference>> logger =null)
                                    : base(context, hubKeyHashFunction,logger) {
    }

    public override TEntity CalculateHashes(TEntity entity) {
      if(entity.Reference == null || entity.Reference.CompareTo(default(THubReference))== 0)
        entity.Reference = HashFunction(entity.GetBusinessKeyString());
      return entity;
    }

    protected override void DBSetAddEntry(TEntity entity) {
      DbSet.Add(entity);
    }

    public override IEnumerable<TEntity> GetCurrent(
        Expression<Func<TEntity, bool>> filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        string includeProperties = "") {
      IQueryable<TEntity> query = DbSet;

      query = query.GroupBy(t => t.Reference)
        .Select(t => t.OrderByDescending(x => x.LoadDate).FirstOrDefault());

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
  }
}
