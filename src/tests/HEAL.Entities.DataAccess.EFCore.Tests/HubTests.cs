using System;
using System.Collections.Generic;
using HEAL.Entities.DataAccess.EFCore.Dwh;
using HEAL.Entities.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage;
using Xunit;
using System.Linq;
using HEAL.Entities.DataAccess.EFCore.Dwh.DataVaultV2;
using HEAL.Entities.Utils.Hashing;
using HEAL.Entities.DataAccess.EFCore.Caching;

namespace HEAL.Entities.DataAccess.EFCore.Tests {
  [Collection("Sequential")]
  public class HubTests {

    private readonly List<H_TestHub_Default> testInstances = new List<H_TestHub_Default>() {
      new H_TestHub_Default("DefaultHubTests0001"),
      new H_TestHub_Default("DefaultHubTests0002"),
      new H_TestHub_Default("DefaultHubTests0003"),
      new H_TestHub_Default("DefaultHubTests0004"),
      new H_TestHub_Default("DefaultHubTests0005"),
      new H_TestHub_Default("DefaultHubTests0006")
    };
    private H_TestHub_Default AdditionalTest = new H_TestHub_Default("DefaultHubTests0007");

    [Fact]
    public void EmptySetsYieldNoResults() {
      using (var context = TestDwhDbContext.CreateDWHDbContext()) {
        var testRepo = new HubRepository<H_TestHub_Default>(context, HashFunctions.MD5, null);

        var testList = testRepo.Get();
        Assert.Empty(testList);

        var test = testRepo.GetByKey("UnknownId");
        Assert.Null(test);
      }
    }

    [Fact]
    public void TestQueriesAndInsert() {
      using (var context = TestDwhDbContext.CreateDWHDbContext()) {
        var testRepo = new HubRepository<H_TestHub_Default>(context, HashFunctions.MD5, null);
        var loadDate = DateTime.Now;

        //Bulk insert
        testRepo.Insert(testInstances, loadDate);

        var testList = testRepo.Get();
        Assert.NotEmpty(testList);
        Assert.Equal(testInstances.Count, testList.Count());
        Assert.Equal(testInstances.Count, testRepo.Count());

        var test = testRepo.GetByKey(AdditionalTest.PrimaryKey);
        Assert.Null(test);

        //additional insert
        testRepo.Insert(AdditionalTest, loadDate);

        testList = testRepo.Get();
        Assert.NotEmpty(testList);
        Assert.Equal(testInstances.Count + 1, testList.Count());
      }
    }


    [Fact]
    public void KeyCalculation() {
      using (var context = TestDwhDbContext.CreateDWHDbContext()) {
        var testRepo = new HubRepository<H_TestHub_Default>(context, HashFunctions.MD5, null, DVKeyCaching.Disabled);
        var loadDate = DateTime.Now;

        //single insert
        var test = testRepo.Insert(AdditionalTest, loadDate);
        Assert.Equal(HashFunctions.MD5(test.GetBusinessKeyString()), test.PrimaryKey);
        Assert.Equal(HashFunctions.MD5(test.GetBusinessKeyString()), testRepo.CalculateHashes(test).PrimaryKey);
      }
    }

    [Fact]
    public void HubKeyCachingPreventsInsert() {
      using (var context = TestDwhDbContext.CreateDWHDbContext()) {
        var cache = new BasePrimaryKeyCache();
        var testRepo = new HubRepository<H_TestHub_Default>(context, HashFunctions.MD5, cache, DVKeyCaching.Enabled);
        var loadDate = DateTime.Now;

        //single insert
        testRepo.Insert(AdditionalTest, loadDate);

        // same key again
        testRepo.Insert(AdditionalTest, loadDate);

        var list = testRepo.Get(x => x.TestNr == AdditionalTest.TestNr);
        Assert.NotNull(list);
        Assert.Single(list);
      }
    }

    [Fact]
    public void KeyCachingCanBeSharedByAllInstances() {
      using (var context = TestDwhDbContext.CreateDWHDbContext()) {
        var cache = new BasePrimaryKeyCache();
        var testRepo = new HubRepository<H_TestHub_Default>(context, HashFunctions.MD5, cache, DVKeyCaching.Enabled);
        var loadDate = DateTime.Now;

        //single insert
        testRepo.Insert(AdditionalTest, loadDate);

        // same key again
        testRepo.Insert(AdditionalTest, loadDate);

        var list = testRepo.Get(x => x.TestNr == AdditionalTest.TestNr);
        Assert.NotNull(list);
        Assert.Single(list);

        //new repository created -> cache is initialized from memory context
        // -> cache already kontains the key
        testRepo = new HubRepository<H_TestHub_Default>(context, HashFunctions.MD5, cache, DVKeyCaching.Enabled);

        // same key again  -> insert is prevented
        testRepo.Insert(AdditionalTest, loadDate);

        list = testRepo.Get(x => x.TestNr == AdditionalTest.TestNr);
        Assert.NotNull(list);
        Assert.Single(list);
      }
    }
  }

}

