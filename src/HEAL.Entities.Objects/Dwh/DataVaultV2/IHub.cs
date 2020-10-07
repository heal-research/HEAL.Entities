using System;
using System.Collections;
using System.Text;

namespace HEAL.Entities.Objects.Dwh.DataVaultV2 {
  /// <summary>
  /// default implementation of Data Vault V2 and <see cref="IHub{TPKey}"/> for primary key hashes stored as string
  /// </summary>
  public interface IHub : IHub<string> { }

}
