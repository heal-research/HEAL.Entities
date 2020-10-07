using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using HEAL.Entities.Objects;
using HEAL.Entities.Objects.Dwh.DataVaultV2;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HEAL.Entities.DataAccess.EFCore.Dwh.DataVaultV2 {
  public static class EntityMappingExtensions {
    /// <summary>
    /// Mapping the single default primary Key Object to the Tables 
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity</typeparam>
    /// <typeparam name="TKey">Type of the primary key</typeparam>
    /// <param name="thiz">the entity builder instance</param>
    public static void UseIdentityColumn<TEntity, TKey>(this EntityTypeBuilder<TEntity> thiz)
        where TEntity : class, IDomainObject<TKey>
        where TKey : IComparable<TKey> {
      thiz.HasKey(e => e.PrimaryKey);
    }

    /// <summary>
    /// Creates a the Table,Schema and Primary key mapping for the given entity
    /// Use this method for SINGLE Primary Key columns
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <param name="thiz">the entitie's ...TypeBuilder instance</param>
    /// <param name="tableName">name of the db table</param>
    /// <param name="schemaName">name of the schema the table resided in</param>
    /// <param name="primaryKeyColumn">name of the single primary key column</param>
    /// <returns></returns>
    public static PropertyBuilder<TKey> UseDomainObjectTable<TEntity, TKey>(this EntityTypeBuilder<TEntity> thiz, string tableName, string schemaName = "dbo", string primaryKeyColumn = "Id")
        where TEntity : class, IDomainObject<TKey>
        where TKey : IComparable<TKey> {
      thiz.UseIdentityColumn<TEntity, TKey>();
      thiz.ToTable(tableName, schemaName);

      return thiz.Property(b => b.PrimaryKey).HasColumnName(primaryKeyColumn);
    }

    /// <summary>
    /// Mapping the single default primary Key Object to the Tables 
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity</typeparam>
    /// <typeparam name="TKey">Type of the primary key</typeparam>
    /// <param name="thiz">the entity builder instance</param>
    /// <param name="primaryKeyObjects">If the primary key is made up of multiple properties then specify an anonymous type including the properties (post => new { post.Title, post.BlogId }).</param>
    public static void UseIdentityColumn<TEntity, TKey>
                            (this EntityTypeBuilder<TEntity> thiz
                            , Expression<Func<TEntity, object>> primaryKeyObjects)
                      where TEntity : class, IDomainObject<TKey>
                      where TKey : IComparable<TKey> {
      thiz.HasKey(primaryKeyObjects);
    }

    /// <summary>
    /// Creates a the Table,Schema and Primary key mapping for the given entity
    /// Use this method for COMPOSITE Primary Key columns
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <param name="thiz">the entitie's ...TypeBuilder instance</param>
    /// <param name="tableName">name of the db table</param>
    /// <param name="schemaName">name of the schema the table resided in</param>
    /// <param name="primaryKeyColumn">name of the single primary key column</param>
    public static void UseDomainObjectTable<TEntity, TKey>
                      (this EntityTypeBuilder<TEntity> thiz
                      , string tableName
                      , string schemaName
                      , Expression<Func<TEntity, object>> primaryKeyObjects)
                where TEntity : class, IDomainObject<TKey>
                where TKey : IComparable<TKey> {
      thiz.UseIdentityColumn<TEntity, TKey>(primaryKeyObjects);
      thiz.ToTable(tableName, schemaName);
    }

  }
}
