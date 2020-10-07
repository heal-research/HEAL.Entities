using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HEAL.Entities.DataAccess.Abstractions;
using HEAL.Entities.DataAccess.EPPlus.AttributeConfiguration.Tests;
using HEAL.Entities.DataAccess.Excel;
using HEAL.Entities.DataAccess.Excel.Abstractions;
using HEAL.Entities.Objects.Dwh.DataVaultV2;
using HEAL.Entities.Objects.Excel;
using Xunit;

namespace HEAL.Entities.DataAccess.EPPlus.ContextConfiguration.Tests {

  public class TestExcelContext : ExcelContext {

    public TestExcelContext(ExcelOptions options) : base(options) {
    }

    protected override void OnCreating(ExcelModelBuilder modelBuilder) {
      modelBuilder.Entity<DomainObject_FluentApi>(Configure_ExcelDomainObjectConfiguration);
    }

    private void Configure_ExcelDomainObjectConfiguration(ExcelEntityBuilder<DomainObject_FluentApi> builder) {
      builder.AuditRowId(x => x.PrimaryKey);

      builder.Property(x => x.Prename)
        .Column(ExcelColumnEnum.A)
        .HasHeaderName("First Name");

      builder.Property(x => x.Surname)
        .Column(2)
        .HasHeaderName("Last Name");

      builder.Property(x => x.Age)
        .Column("C");

      builder.Property(x => x.Occupation)
        .Column(ExcelColumnEnum.D)
        .WithDefault(UnitTestMetadata.DefaultOccupation);

      builder.Property(x => x.Salary)
        .Column(ExcelColumnEnum.E);

      builder.Property(x => x.Married)
        .Column(ExcelColumnEnum.F)
        .UseCustomParser((x)=> x=="Married" ? true : false);

      builder.Property(x => x.HasChildren)
        .Column(ExcelColumnEnum.G)
        .UseCustomParser(new BinaryBooleanParser());

      builder.Property(x => x.Hobbies)
        .Column(ExcelColumnEnum.H)
        .UseCustomParser((s) => string.IsNullOrEmpty(s) ? new string[0] : s.Split(',').Select(x => x.Trim()).ToArray());
    }
  }
}
