using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HEAL.Entities.Objects.Excel;
using HEAL.Entities.Utils.Enumerables;

namespace HEAL.Entities.DataAccess.Excel {

  /// <summary>
  /// Excel context is used to track the mapping configurations of entities and other
  /// <see cref="ExcelOptions"/> that can be defined for all domain objects of this context
  /// </summary>
  public class ExcelContext : IDisposable {

    /// <summary>
    /// use this contructor to create a new context. Also for usage of the <see cref="ExcelColumnConfigurationAttribute"/> variant.
    /// </summary>
    /// <param name="options">leave null to apply default configurations</param>
    public ExcelContext(ExcelOptions options = null) {
      if (options == null)
        options = new ExcelOptions();
      Options = options;
      ModelBuilder = new ExcelModelBuilder();
      OnCreating(ModelBuilder);
      ValidateModel();
    }

    /// <summary>
    /// the model mapping information for consistency and completions
    /// </summary>
    internal void ValidateModel() {
      foreach (var entityPair in ModelBuilder.Entities) {
        var entityBuilder = entityPair.Value;

        //validate an entity
        ValidateEntityBuilder(entityBuilder);

        foreach (var propertyBuilderPair in entityBuilder.ColumnConfigurations) {
          var propertyBuilder = propertyBuilderPair.Value;

          //validate one property of an entity
          ValidatePropertyBuilder(propertyBuilder);
        }
      }
    }

    private void ValidatePropertyBuilder(ExcelPropertyBuilder propertyBuilder) {
    }

    private void ValidateEntityBuilder(ExcelEntityBuilder entityBuilder) {
      DataPropertiesMustHaveUniqueColumnAdresses(entityBuilder);
    }

    private static void DataPropertiesMustHaveUniqueColumnAdresses(ExcelEntityBuilder entityBuilder) {
      var adresses = entityBuilder.ColumnConfigurations.Values.Where(x => x.Configuration.AuditPropertyType == ExcelAuditProperties.NO_AUDIT_FIELD)
                          .Select(x => x.Configuration.ColumnIndex);

      if (adresses.Any(x => x == default)) {
        throw new Exception($"Some columns where configured without a column index." +
          $" Use '{nameof(ExcelPropertyBuilder<object>)}.{nameof(ExcelPropertyBuilder<object>.Column)}' to set the column index.");
      }

      var duplicatedAddresses = adresses.CountDistinct().Where(x => x.Value > 1).Select(x => ExcelPropertyConfiguration.ColumnAdress(x.Key));
      if (duplicatedAddresses.Count() > 0) {
        throw new Exception($"The excel column adresses [{string.Join(",", duplicatedAddresses)}] where defined multiple times.");
      }
    }

    /// <summary>
    /// override this method in your derived class to call <see cref="ExcelModelBuilder.Entity{TEntity}(Action{ExcelEntityBuilder{TEntity}})"/>
    /// to start tracking an entity and configure it's properties and associated excel column
    /// </summary>
    /// <param name="modelBuilder"></param>
    protected virtual void OnCreating(ExcelModelBuilder modelBuilder) { }

    /// <summary>
    /// builder for the whole model tracked by this context
    /// </summary>
    internal ExcelModelBuilder ModelBuilder { get; set; }

    /// <summary>
    /// convenience method for configuration access
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns></returns>
    internal IDictionary<string, ExcelPropertyBuilder> GetPropertyBuilders<TEntity>() {
      return ObtainEntityFromDictionary<TEntity>().ColumnConfigurations;
    }

    /// <summary>
    /// guarded extraction of dictionary value
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns></returns>
    internal ExcelEntityBuilder ObtainEntityFromDictionary<TEntity>() {
      if (ModelBuilder.Entities.TryGetValue(typeof(TEntity), out ExcelEntityBuilder builder)) {
        return builder;
      }

      throw new ArgumentException($"The provided type '{typeof(TEntity)}' was not configured as a mapped entity in the excel context." +
        $" Call the extension method of'{nameof(ExcelContext)}.{nameof(ExcelContextExtensions.BuildAttributedEntity)}' or the " +
        $"FluentAPI function of '{nameof(ExcelModelBuilder)}.{nameof(ExcelModelBuilder.Entity)}' to start configuring an entity.");
    }

    /// <summary>
    /// convenience method for builder access
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns></returns>
    internal ExcelEntityBuilder<TEntity> GetExcelEntityTypeBuilder<TEntity>()
        where TEntity : class {
      return (ExcelEntityBuilder<TEntity>)ObtainEntityFromDictionary<TEntity>();
    }

    /// <summary>
    /// returns a clone of the internal excel options. change of parameters is not possible.
    /// </summary>
    public ExcelOptions GetExcelOptions {
      get { return (ExcelOptions)Options.Clone(); }
    }
    internal ExcelOptions Options = new ExcelOptions();

    /// <summary>
    /// disposes of the context and its options
    /// </summary>
    public void Dispose() {
      Options?.Dispose();
    }

  }
}
