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
  public class LinkTests {


    private static readonly H_TestHub_Default hub1 = new H_TestHub_Default("DefaultLinkTests0001");
    private static readonly H_TestHub_Default hub2 = new H_TestHub_Default("DefaultLinkTests0002");
    private static readonly H_TestHub_Default hub3 = new H_TestHub_Default("DefaultLinkTests0003");
    private static readonly H_TestHub_Default hub4 = new H_TestHub_Default("DefaultLinkTests0004");
    private static readonly H_TestHub_Default hub5 = new H_TestHub_Default("DefaultLinkTests0005");
    private static readonly H_TestHub_Default hub6 = new H_TestHub_Default("DefaultLinkTests0006");
    private static readonly List<H_TestHub_Default> hubs
                      = new List<H_TestHub_Default>() {
                                                     hub1,
                                                     hub2,
                                                     hub3,
                                                     hub4,
                                                     hub5,
                                                     hub6
                                                  };


    [Fact]
    public void CreateLink() {
      using (var context = TestDwhDbContext.CreateDWHDbContext()) {
        var keyCache = new BasePrimaryKeyCache();
        var hubRepo = new HubRepository<H_TestHub_Default>(context, HashFunctions.MD5, null);
        var linkRepo = new LinkRepository<L_TestLink_Default>(context, HashFunctions.MD5, keyCache, DVKeyCaching.Enabled);
        var loadDate = DateTime.Now;

        hubRepo.Insert(hubs, loadDate);

        linkRepo.Insert(new L_TestLink_Default(hub1.PrimaryKey, hub2.PrimaryKey), loadDate);
        linkRepo.Insert(new L_TestLink_Default(hub1.PrimaryKey, hub2.PrimaryKey), loadDate);
      }
    }
    [Fact]
    public void KeyCalculation() {
      using (var context = TestDwhDbContext.CreateDWHDbContext()) {
        var keyCache = new BasePrimaryKeyCache();
        var hubRepo = new HubRepository<H_TestHub_Default>(context, HashFunctions.MD5, null);
        var linkRepo = new LinkRepository<L_TestLink_Default>(context, HashFunctions.MD5, keyCache, DVKeyCaching.Enabled);
        var loadDate = DateTime.Now;

        hubRepo.Insert(hubs, loadDate);

        L_TestLink_Default link = new L_TestLink_Default(hub1.PrimaryKey, hub2.PrimaryKey);
        //single insert
        var insertedLink = linkRepo.Insert(link, loadDate);
        Assert.Equal(HashFunctions.MD5(link.GetBusinessKeyString()), insertedLink.PrimaryKey);
      }
    }

    [Fact]
    public void LinkKeyCaching() {
      //Caching enabled -> same key is ignored
      using (var context = TestDwhDbContext.CreateDWHDbContext()) {
        var keyCache = new BasePrimaryKeyCache();
        var hubRepo = new HubRepository<H_TestHub_Default>(context, HashFunctions.MD5, null);
        var linkRepo = new LinkRepository<L_TestLink_Default>(context, HashFunctions.MD5, keyCache, DVKeyCaching.Enabled);
        var loadDate = DateTime.Now;

        hubRepo.Insert(hubs, loadDate);

        //single insert
        var insertedLink = linkRepo.Insert(new L_TestLink_Default(hub1.PrimaryKey, hub2.PrimaryKey), loadDate);
        // same key again
        var insertedLink2 = linkRepo.Insert(new L_TestLink_Default(hub1.PrimaryKey, hub2.PrimaryKey), loadDate);

        Assert.Equal(insertedLink.PrimaryKey, insertedLink2.PrimaryKey);

        var queriedLinks = linkRepo.Get();

        Assert.NotNull(queriedLinks);
        Assert.Single(queriedLinks);
      }

      //caching disabled -> insert throws exception
      using (var context = TestDwhDbContext.CreateDWHDbContext()) {
        var hubRepo = new HubRepository<H_TestHub_Default>(context, HashFunctions.MD5, null);
        var linkRepo = new LinkRepository<L_TestLink_Default>(context, HashFunctions.MD5, null, DVKeyCaching.Disabled);
        var loadDate = DateTime.Now;

        hubRepo.Insert(hubs, loadDate);

        //single insert
        var insertedLink = linkRepo.Insert(new L_TestLink_Default(hub1.PrimaryKey, hub2.PrimaryKey), loadDate);
        // same key again
        Assert.Throws<InvalidOperationException>(() => linkRepo.Insert(new L_TestLink_Default(hub1.PrimaryKey, hub2.PrimaryKey), loadDate));
      }
    }
    [Fact]
    public void KeyCachingIsSharedByAllInstances() {
      using (var context = TestDwhDbContext.CreateDWHDbContext()) {
        var keyCache = new BasePrimaryKeyCache();
        var hubRepo = new HubRepository<H_TestHub_Default>(context, HashFunctions.MD5, null);
        var linkRepo = new LinkRepository<L_TestLink_Default>(context, HashFunctions.MD5, keyCache, DVKeyCaching.Enabled);
        var loadDate = DateTime.Now;

        hubRepo.Insert(hubs, loadDate);

        //single insert
        var insertedLink = linkRepo.Insert(new L_TestLink_Default(hub1.PrimaryKey, hub2.PrimaryKey), loadDate);

        //new repository created -> cache is initialized from memory context
        // -> cache already kontains the key
        linkRepo = new LinkRepository<L_TestLink_Default>(context, HashFunctions.MD5, keyCache, DVKeyCaching.Enabled);

        // same key again -> insert prevented
        var insertedLink2 = linkRepo.Insert(new L_TestLink_Default(hub1.PrimaryKey, hub2.PrimaryKey), loadDate);

        var list = linkRepo.Get();
        Assert.NotNull(list);
        Assert.Single(list);
      }
    }


  }
}
