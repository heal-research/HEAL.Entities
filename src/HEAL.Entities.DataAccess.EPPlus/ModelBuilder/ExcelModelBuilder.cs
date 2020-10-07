using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using HEAL.Entities.Objects.Excel;

namespace HEAL.Entities.DataAccess.Excel {

  /// <summary>
  /// Main builder of the <see cref="ExcelContext"/> provides methods to track new domain objects
  /// </summary>
  public class ExcelModelBuilder {

    public ExcelModelBuilder() {
    }


    /// <summary>
    /// tracks entity of type <typeparamref name="TEntity"/> in the context. Creates a new
    /// <see cref="ExcelEntityBuilder{TEntity}"/> and calls the passed configuration method
    /// </summary>
    /// <typeparam name="TEntity">type of the domain object</typeparam>
    /// <param name="buildAction">gets called by this method and contains the builder configuration</param>
    public virtual void Entity<TEntity>(Action<ExcelEntityBuilder<TEntity>> buildAction)
      where TEntity : class {
      var entityBuilder = new ExcelEntityBuilder<TEntity>();
      try {
        Entities.Add(typeof(TEntity), entityBuilder);
      }
      catch (ArgumentException e) {
        throw new ArgumentException($"The type '{typeof(TEntity)}' was already configured as a mapped entity in the excel context.");
      }
      
      buildAction(entityBuilder);
    }

    /// <summary>
    /// List of all entities and their builders tracked by the <see cref="ExcelContext"/>
    /// </summary>
    internal IDictionary<Type, ExcelEntityBuilder> Entities { get; set; } = new Dictionary<Type, ExcelEntityBuilder>();
  }
}
