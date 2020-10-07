using System;
using System.Collections.Generic;
using HEAL.Entities.DataAccess.EFCore.Dwh;
using HEAL.Entities.Utils.Hashing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage;
using Xunit;
using System.Linq;
using HEAL.Entities.DataAccess.EFCore.Dwh.DataVaultV2;
using HEAL.Entities.DataAccess.EFCore.Caching;

namespace HEAL.Entities.DataAccess.EFCore.Tests {
  [Collection("Sequential")]
  public class AuditReferencingLoadInfo {
    private const string TestHubBusinessKey1 = "AuditReferencingLoadInfo0001";
    private const string TestHubBusinessKey2 = "AuditReferencingLoadInfo0002";

    private LoadInformation LoadInfo1 = new LoadInformation("RecordSource1", "LoadedBy1");
    private LoadInformation LoadInfo2 = new LoadInformation("RecordSource2", "LoadedBy2");


    private H_TestHub_References TestHub1 = new H_TestHub_References(TestHubBusinessKey1);
    private H_TestHub_References TestHub2 = new H_TestHub_References(TestHubBusinessKey2);

    private L_TestLink_References Link1 = new L_TestLink_References(HashFunctions.MD5(TestHubBusinessKey1)
                                                                  , HashFunctions.MD5(TestHubBusinessKey2));


    private S_TestSatellite_References Hub1Satellite1 = new S_TestSatellite_References(1, TestHubBusinessKey1);
    private S_TestSatellite_References Hub1Satellite2 = new S_TestSatellite_References(2, TestHubBusinessKey1);
    private S_TestSatellite_References Hub2Satellite1 = new S_TestSatellite_References(3, TestHubBusinessKey2);
    private S_TestSatellite_References Hub2Satellite2 = new S_TestSatellite_References(4, TestHubBusinessKey2);
    private S_TestSatellite_References Hub2Satellite3 = new S_TestSatellite_References(5, TestHubBusinessKey2);



    [Fact]
    public void TestQueriesAndInsert() {
      using (var context = TestDwhDbContext.CreateDWHDbContext()) {
        var referenceRepo = new CRUDRepository<LoadInformation,long>(context);

        var hubRepo = new HubRepository<H_TestHub_References>(context, HashFunctions.MD5, null);
        var satelliteRepo = new SatelliteRepository<S_TestSatellite_References>(context, HashFunctions.MD5);
        var linkRepo = new LinkRepository<L_TestLink_References>(context, HashFunctions.MD5,null, DVKeyCaching.Disabled);

        var loadDate = DateTime.Now;

        // REFERENCE ENTRY - inserting and data persistence tests
        var load1Key = referenceRepo.Insert(LoadInfo1);
        var load2Key = referenceRepo.Insert(LoadInfo2);

        var hubList = referenceRepo.Get();
        Assert.NotEmpty(hubList);
        Assert.Equal(2, hubList.Count());
        Assert.False(load1Key == default);
        Assert.False(load2Key == default);

        var loadInfo = referenceRepo.Get(x => x.PrimaryKey == load2Key).Single();

        Assert.Equal(LoadInfo2.RecordSource, loadInfo.RecordSource);                                        
        Assert.Equal(LoadInfo2.LoadedBy, loadInfo.LoadedBy);

        // HUB - referencing
        TestHub1 = hubRepo.Insert(TestHub1, loadDate, loadReference: LoadInfo1);
        TestHub2 = hubRepo.Insert(TestHub2, loadDate, loadReference: LoadInfo2);
        Assert.Equal(load1Key, TestHub1.LoadReference);
        Assert.Equal(load2Key, TestHub2.LoadReference);

        // SATELLITE - referencing
        Hub1Satellite1 = satelliteRepo.Insert(Hub1Satellite1, loadDate, loadReference: LoadInfo1);
        Hub2Satellite1 = satelliteRepo.Insert(Hub2Satellite1, loadDate, loadReference: LoadInfo2);

        Assert.Equal(load1Key, Hub1Satellite1.LoadReference);
        Assert.Equal(load2Key, Hub2Satellite1.LoadReference);

        var satelliteList = satelliteRepo.GetCurrent();
        Assert.NotEmpty(satelliteList);
        Assert.Equal(2, satelliteList.Count());


        // LINK - referencing
        Link1 = linkRepo.Insert(Link1, loadDate, loadReference: LoadInfo1);

        Assert.Equal(load1Key, Link1.LoadReference);

        Link1 = linkRepo.Get().Single();
        Assert.Equal(load1Key, Link1.LoadReference);
      }
    }
  }
}
