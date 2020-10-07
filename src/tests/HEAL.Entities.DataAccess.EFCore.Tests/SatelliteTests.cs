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
  public class SatelliteTests {
    private const string TestHubBusinessKey1 = "DefaultSatelliteTests0001";
    private const string TestHubBusinessKey2 = "DefaultSatelliteTests0002";

    private H_TestHub_Default TestHub1 = new H_TestHub_Default(TestHubBusinessKey1);
    private H_TestHub_Default TestHub2 = new H_TestHub_Default(TestHubBusinessKey2);

    private S_TestSatellite_Default Hub1Satellite1 = new S_TestSatellite_Default(1, TestHubBusinessKey1);
    private S_TestSatellite_Default Hub1Satellite2 = new S_TestSatellite_Default(2, TestHubBusinessKey1);
    private S_TestSatellite_Default Hub2Satellite1 = new S_TestSatellite_Default(3, TestHubBusinessKey2);
    private S_TestSatellite_Default Hub2Satellite2 = new S_TestSatellite_Default(4, TestHubBusinessKey2);
    private S_TestSatellite_Default Hub2Satellite3 = new S_TestSatellite_Default(5, TestHubBusinessKey2);


    [Fact]
    public void EmptySetsYieldNoResults() {
      using (var context = TestDwhDbContext.CreateDWHDbContext()) {
        var testRepo = new SatelliteRepository<S_TestSatellite_Default>(context, HashFunctions.MD5);

        var testList = testRepo.Get();
        Assert.Empty(testList);

        testList = testRepo.GetAll();
        Assert.Empty(testList);
      }
    }

    [Fact]
    public void TestQueriesAndInsert() {
      using (var context = TestDwhDbContext.CreateDWHDbContext()) {
        var hubRepo = new HubRepository<H_TestHub_Default>(context, HashFunctions.MD5, null);
        var satelliteRepo = new SatelliteRepository<S_TestSatellite_Default>(context, HashFunctions.MD5);

        var loadDate = DateTime.Now;

        hubRepo.Insert(TestHub1, loadDate);
        hubRepo.Insert(TestHub2, loadDate);

        var hubList = hubRepo.Get();
        Assert.NotEmpty(hubList);
        Assert.Equal(2, hubList.Count());


        satelliteRepo.Insert(Hub1Satellite1, loadDate);
        satelliteRepo.Insert(Hub2Satellite1, loadDate);

        var satelliteList = satelliteRepo.GetCurrent();
        Assert.NotEmpty(satelliteList);
        Assert.Equal(2, satelliteList.Count());

        //increment load date to simulate later insert of new data slice
        loadDate = loadDate.AddDays(1);

        satelliteRepo.Insert(Hub1Satellite2, loadDate);
        satelliteRepo.Insert(Hub2Satellite2, loadDate);

        satelliteList = satelliteRepo.GetCurrent();
        Assert.NotEmpty(satelliteList);
        Assert.Equal(2, satelliteList.Count());

        var satHub1 = satelliteList.Where(sat => sat.TestNr == TestHubBusinessKey1).Single();
        var satHub2 = satelliteList.Where(sat => sat.TestNr == TestHubBusinessKey2).Single();

        Assert.Equal(Hub1Satellite2, satHub1);
        Assert.Equal(Hub2Satellite2, satHub2);


        // a total of 4 should now be stored in the db
        satelliteList = satelliteRepo.GetAll();
        Assert.NotEmpty(satelliteList);
        Assert.Equal(4, satelliteList.Count());
      }
    }


    [Fact]
    public void KeyCalculation() {
      using (var context = TestDwhDbContext.CreateDWHDbContext()) {
        var hubRepo = new HubRepository<H_TestHub_Default>(context, HashFunctions.MD5, null, DVKeyCaching.Disabled);
        var satelliteRepo = new SatelliteRepository<S_TestSatellite_Default>(context, HashFunctions.MD5);

        var loadDate = DateTime.Now;

        //single insert
        var testHub = hubRepo.Insert(TestHub1, loadDate);
        Assert.Equal(HashFunctions.MD5(testHub.GetBusinessKeyString()), testHub.PrimaryKey);
        Assert.Equal(HashFunctions.MD5(testHub.GetBusinessKeyString()), hubRepo.CalculateHashes(testHub).PrimaryKey);

        //single insert
        var testSatellite = satelliteRepo.Insert(Hub1Satellite1, loadDate);
        Assert.Equal(HashFunctions.MD5(testHub.GetBusinessKeyString()), testSatellite.Reference);
        Assert.Equal(HashFunctions.MD5(testHub.GetBusinessKeyString()), testSatellite.PrimaryKey.Reference);
      }
    }

    [Fact]
    public void DuplicatePrimaryKeyException() {
      using (var context = TestDwhDbContext.CreateDWHDbContext()) {
        var satelliteRepo = new SatelliteRepository<S_TestSatellite_Default>(context, HashFunctions.MD5);

        var loadDate = DateTime.Now;

        //single insert
        satelliteRepo.Insert(Hub1Satellite1, loadDate);

        // same key (same loaddate) again
        Assert.Throws<InvalidOperationException>(() => satelliteRepo.Insert(Hub1Satellite2, loadDate));
      }
    }
  }
}
