using HEAL.Entities.DataAccess.Abstractions;
using HEAL.Entities.Objects;
using HEAL.Entities.Objects.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace HEAL.Entities.DataAccess.Excel.Abstractions {
  /// <summary>
  /// Excel Row Id is used as unique identifyer of an excel domain object
  /// </summary>
  /// <typeparam name="TEntity"></typeparam>
  public interface IExcelRepository<TEntity, TKey> : IReadRepository<TEntity, TKey>, IDisposable
      where TEntity : IDomainObject<TKey>
      where TKey : IComparable<TKey> {
    TEntity GetByRowId(int id);
    /// <summary>
    /// use the currently configured excel context and its mapped model for the current
    /// <typeparamref name="TEntity"/> on another source file or configure the source file at a later programm point
    /// </summary>
    /// <param name="excelFileStream"></param>
    /// <param name="worksheetName"></param>
    /// <param name="filePasword"></param>
    void SetExcelDataSource(Stream excelFileStream, string worksheetName = default, string filePasword = default);
  }
}
