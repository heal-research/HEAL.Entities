using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using HEAL.Entities.Objects.Excel;
using HEAL.Entities.Utils.Enumerables;

namespace HEAL.Entities.DataAccess.Excel {

  public static class ExcelContextExtensions {

    /// <summary>
    /// Extension method to <see cref="ExcelContext"/> to add domain objects that use the Attribute 
    /// interface <see cref="ExcelColumnConfigurationAttribute"/> for column configuration
    /// </summary>
    /// <typeparam name="TEntity">type of entity configured with attributes</typeparam>
    /// <param name="context">the context that should track the entity</param>
    /// <returns></returns>
    public static ExcelContext BuildAttributedEntity<TEntity>(this ExcelContext context)
        where TEntity : class, new() {
      var typeBuilder = new ExcelEntityBuilder<TEntity>();
      var typeBuilderType = typeBuilder.GetType();

      var entityType = typeof(TEntity);
      var entityProperties = entityType.GetProperties();

      //iterate through all properties of the entity
      foreach (var targetProp in entityProperties) {
        var allCustomAttributes = targetProp.GetCustomAttributes(true);

        //property is marked as audit attribute
        var excelAuditAttribute = allCustomAttributes.FirstOrDefault(x => x.GetType() == typeof(ExcelAuditAttribute)) as ExcelAuditAttribute;
        if (excelAuditAttribute != null) {
          switch (excelAuditAttribute.AuditProperty) {
            case ExcelAuditProperties.RowId:
              //use the fluent api to track the audit property
              typeBuilder.RowIdWithPropertyName(targetProp.Name);
              break;
            default:
              throw new NotSupportedException($"The audit property type '{excelAuditAttribute.AuditProperty}' is not supported by the method '{nameof(ExcelContextExtensions.BuildAttributedEntity)}'.");
          }
          continue;
        }

        //property is marked with a data attribute
        var excelConfigAttribute = allCustomAttributes.FirstOrDefault(x => x.GetType() == typeof(ExcelColumnConfigurationAttribute)) as ExcelColumnConfigurationAttribute;
        if (excelConfigAttribute != null) {
          //add the property as via the generic api method 
          //this requires ad-hoc reflection based method construction from generic to type of property
          var genericMethod= typeBuilderType.GetMethod(nameof(ExcelEntityBuilder<TEntity>.PropertyWithPropertyName), 
                                                      BindingFlags.Instance | BindingFlags.NonPublic);
          var typedGenericMethod = genericMethod.MakeGenericMethod(targetProp.PropertyType);
          var propertyBuilder = typedGenericMethod.Invoke(typeBuilder, new object[] { targetProp.Name });
          
          ///configuration property can be accessed from the non generic base class
          ((ExcelPropertyBuilder)propertyBuilder).Configuration = excelConfigAttribute.Configuration;
        }

      }

      try {
        context.ModelBuilder.Entities.Add(entityType, typeBuilder);
      }
      catch(ArgumentException e) {
        throw new ArgumentException($"The type '{typeof(TEntity)}' was already configured as a mapped entity in the excel context.");
      }
      context.ValidateModel();

      return context;
    }

    /// <summary>
    /// add a function that detects end of data for a specific entity
    /// </summary>
    /// <typeparam name="TEntity">parsed entity type</typeparam>
    /// <param name="context">the excel context tracking the entity</param>
    /// <param name="endDelimiter">function that detects non-data rows</param>
    /// <returns></returns>
    public static ExcelContext AddEndDelimiter<TEntity>(this ExcelContext context, Expression<Func<TEntity, bool>> endDelimiter)
          where TEntity : class, new() {
      var typeBuilder = context.GetExcelEntityTypeBuilder<TEntity>();
      typeBuilder.LastRowWhen(endDelimiter);
      return context;
    }
  }
}
