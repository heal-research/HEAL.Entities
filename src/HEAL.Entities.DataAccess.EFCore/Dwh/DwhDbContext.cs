using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace HEAL.Entities.DataAccess.EFCore.Dwh {
  /// <summary>
  /// Derives directly from <see cref="DbContext"/> and sets default configurations for usage of EF DbContext in DWH environment
  ///  - ChangeTracker.AutoDetectChangesEnabled = false; //remember to call <see cref="ChangeTracker.DetectChanges()"/> if an update is necesary
  /// </summary>
  public class DwhDbContext : DbContext {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="options">the options for this context</param>
    /// <param name="commandExecutionTimeOut">sets timeout for command execution. in SECONDS</param>
    public DwhDbContext(DbContextOptions options, int commandExecutionTimeOut) : base(options) {
      this.ChangeTracker.AutoDetectChangesEnabled = false;
      this.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
      if (this.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
        this.Database.SetCommandTimeout(commandExecutionTimeOut);
    }
  }
}
