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
  public class AuditStoringLoadInfo {
    private const string TestHubBusinessKey1 = "AuditStoringLoadInfo0001";
    private const string TestHubBusinessKey2 = "AuditStoringLoadInfo0002";

    private LoadInformation LoadInfo1 = new LoadInformation("RecordSource1", "LoadedBy1");
    private LoadInformation LoadInfo2 = new LoadInformation("RecordSource2", "LoadedBy2");

    private H_TestHub_Stores TestHub1 = new H_TestHub_Stores(TestHubBusinessKey1);
    private H_TestHub_Stores TestHub2 = new H_TestHub_Stores(TestHubBusinessKey2);

    private L_TestLink_Stores Link1 = new L_TestLink_Stores(HashFunctions.MD5(TestHubBusinessKey1)
                                                                  , HashFunctions.MD5(TestHubBusinessKey2));

    private S_TestSatellite_Stores Hub1Satellite1 = new S_TestSatellite_Stores(1, TestHubBusinessKey1);
    private S_TestSatellite_Stores Hub1Satellite2 = new S_TestSatellite_Stores(2, TestHubBusinessKey1);
    private S_TestSatellite_Stores Hub2Satellite1 = new S_TestSatellite_Stores(3, TestHubBusinessKey2);
    private S_TestSatellite_Stores Hub2Satellite2 = new S_TestSatellite_Stores(4, TestHubBusinessKey2);
    private S_TestSatellite_Stores Hub2Satellite3 = new S_TestSatellite_Stores(5, TestHubBusinessKey2);


    [Fact]
    public void TestQueriesAndInsert() {
      using (var context = TestDwhDbContext.CreateDWHDbContext()) {

        var hubRepo = new HubRepository<H_TestHub_Stores>(context, HashFunctions.MD5, null);
        var satelliteRepo = new SatelliteRepository<S_TestSatellite_Stores>(context, HashFunctions.MD5);
        var linkRepo = new LinkRepository<L_TestLink_Stores>(context, HashFunctions.MD5, null, DVKeyCaching.Disabled);

        var loadDate = DateTime.Now;

        // HUB - storing info
        hubRepo.Insert(TestHub1, loadDate, loadInformation: LoadInfo1);

        var hub = hubRepo.GetByKey(TestHub1.PrimaryKey);

        Assert.Equal(LoadInfo1.RecordSource, hub.RecordSource);
        Assert.Equal(LoadInfo1.LoadedBy, hub.LoadedBy);

        // SATELLITE - storing info
        Hub1Satellite1 = satelliteRepo.Insert(Hub1Satellite1, loadDate, loadInformation: LoadInfo1);

        var sat = satelliteRepo.GetCurrent(s => s.Reference == TestHub1.PrimaryKey).Single();

        Assert.Equal(LoadInfo1.RecordSource, sat.RecordSource);
        Assert.Equal(LoadInfo1.LoadedBy, sat.LoadedBy);

        // LINK - storing info
        Link1 = linkRepo.Insert(Link1, loadDate, loadInformation: LoadInfo1);

        var link = linkRepo.GetByKey(Link1.PrimaryKey);

        Assert.Equal(LoadInfo1.RecordSource, link.RecordSource);
        Assert.Equal(LoadInfo1.LoadedBy, link.LoadedBy);
      }
    }
  }
}
