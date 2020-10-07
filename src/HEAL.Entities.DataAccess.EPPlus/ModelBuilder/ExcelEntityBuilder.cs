using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using HEAL.Entities.Objects.Excel;
using HEAL.Entities.Utils.Reflection;

namespace HEAL.Entities.DataAccess.Excel {

  /// <summary>
  /// Provides methods for the configuration of one entity, i.e. domain object, for the <see cref="ExcelContext"/>
  /// non generic gase class that contains all methods and properties not directly associtated with one entity tyep
  /// </summary>
  public class ExcelEntityBuilder {
    /// <summary>
    /// name of the property and its associated configuration
    /// </summary>
    internal IDictionary<string, ExcelPropertyBuilder> ColumnConfigurations { get; set; } = new Dictionary<string, ExcelPropertyBuilder>();

    /// <summary>
    /// call this method to map one domain object property to create an instance of <see cref="ExcelPropertyBuilder{TProperty}"/>
    /// to further configure this property
    /// </summary>
    /// <typeparam name="TProperty"></typeparam>
    /// <param name="propertyname"></param>
    /// <returns></returns>
    internal ExcelPropertyBuilder<TProperty> PropertyWithPropertyName<TProperty>(string propertyname) {
      if (ColumnConfigurations.TryGetValue(propertyname, out ExcelPropertyBuilder value)){
        if (value.Configuration.AuditPropertyType != ExcelAuditProperties.NO_AUDIT_FIELD)
          throw new ArgumentException($"The property '{propertyname}' is already configured" +
            $" as an audit attribute of category '{value.Configuration.AuditPropertyType}' and " +
            $"cannot be tracked as a normal data property mapping. ");
        return (ExcelPropertyBuilder<TProperty>)value;
      } else {
        var propertyBuilder = new ExcelPropertyBuilder<TProperty>();
        var propertyName = propertyname;
        ColumnConfigurations.Add(propertyName, propertyBuilder);
        return propertyBuilder;
      }
    }

    /// <summary>
    /// call this method to map one domain object property to indicate that this property 
    /// </summary>
    /// <param name="propertyName"></param>
    internal void RowIdWithPropertyName(string propertyName) {
      if (ColumnConfigurations.TryGetValue(propertyName, out ExcelPropertyBuilder value)){
        if (value.Configuration.AuditPropertyType == ExcelAuditProperties.NO_AUDIT_FIELD)
          throw new ArgumentException($"The property '{propertyName}' is already configured" +
            $" as an data attribute and cannot be tracked as an audit attribute additionally.");
      } else {
        var propertyBuilder = new ExcelPropertyBuilder<int>();
        propertyBuilder.Configuration.PropertyName = propertyName;
        propertyBuilder.Configuration.AuditPropertyType = Objects.Excel.ExcelAuditProperties.RowId;
        ColumnConfigurations.Add(propertyName, propertyBuilder);
      }
    }
  }


  /// <summary>
  /// an instance of this class is created by the <see cref="ExcelModelBuilder.Entity{TEntity}(Action{ExcelEntityBuilder{TEntity}})"/>
  /// for the specific entity type that you want to track. <see cref="ExcelEntityBuilder{TEntity}"/> contains all the builder funtions
  /// for the individual properties of an entity
  /// </summary>
  /// <typeparam name="TEntity">type of the tracked entity</typeparam>
  public class ExcelEntityBuilder<TEntity> : ExcelEntityBuilder
      where TEntity : class {

    /// <summary>
    /// function that is called to evaluate if the last data row has already been reached
    /// </summary>
    internal Func<TEntity, bool> LastRowDetector { get; set; }

    /// <summary>
    /// provide a function that is able to detect if the last data row has been reached by the parser
    /// </summary>
    /// <param name="lastRowDetector">function that is able to detect the last row</param>
    /// <returns></returns>
    public ExcelEntityBuilder<TEntity> LastRowWhen(Expression<Func<TEntity, bool>> lastRowDetector) {
      LastRowDetector = lastRowDetector.Compile();
      return this;
    }

    /// <summary>
    /// Build the configuration for one individual property of the entity. Returns an instance of <see cref="ExcelPropertyBuilder{TProperty}"/>
    /// that allows you to configure all property specific configurations like the column address <see cref="ExcelColumnEnum"/>
    /// </summary>
    /// <typeparam name="TProperty">type of the property that is now mapped</typeparam>
    /// <param name="propertySelector">property selector to determine property name and type</param>
    /// <returns></returns>
    public ExcelPropertyBuilder<TProperty> Property<TProperty>(Expression<Func<TEntity, TProperty>> propertySelector) {
      return PropertyWithPropertyName<TProperty>(ReflectionHelpers.GetPropertyInfo(propertySelector).Name);
    }

    /// <summary>
    /// determine which property is to store the excel row id of the individual data row
    /// </summary>
    /// <param name="propertySelector">select an property of type <see cref="int"/> that should store the row id</param>
    public void AuditRowId(Expression<Func<TEntity, int>> propertySelector) {
      var propertyInfo = ReflectionHelpers.GetPropertyInfo(propertySelector);
      var propertyName = propertyInfo.Name; //TODO use the propertySelector to determine propertyname
      RowIdWithPropertyName(propertyName);
    }
  }
}
