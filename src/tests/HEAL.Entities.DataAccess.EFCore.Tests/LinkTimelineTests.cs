using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using HEAL.Entities.DataAccess.EFCore.Dwh.DataVaultV2;
using HEAL.Entities.Utils.Hashing;
using HEAL.Entities.DataAccess.Caching.Abstractions;
using HEAL.Entities.DataAccess.EFCore.Caching;

namespace HEAL.Entities.DataAccess.EFCore.Tests {
  [Collection("Sequential")]
  public class LinkTimelineTests {
    private const int insertCount = 10;
    private TimeSpan OneDay = new TimeSpan(1, 0, 0, 0);
    private TimeSpan TwelfeHours = new TimeSpan(0, 12, 0, 0);
    private static SatelliteRepository<S_LinkTimelineEntry> GetTimelineRepo(TestDwhDbContext context) {
      return new SatelliteRepository<S_LinkTimelineEntry>(context, HashFunctions.MD5);
    }

    private static LinkTimelineRepository<L_Link_Timeline, S_LinkTimelineEntry> GetLinkRepo(TestDwhDbContext context, IPrimaryKeyCache keyCache) {

      return new LinkTimelineRepository<L_Link_Timeline, S_LinkTimelineEntry>(context, HashFunctions.MD5, keyCache, DVKeyCaching.Enabled);
    }

    [Fact]
    public void StoreLinks() {
      using (var context = TestDwhDbContext.CreateDWHDbContext()) {
        var keyCache = new BasePrimaryKeyCache();
        var linkRepo = GetLinkRepo(context, keyCache);
        var loadDate = DateTime.Now;

        var list = new List<L_Link_Timeline>();
        for (int i = 0; i < insertCount; i++) {
          var link = new L_Link_Timeline();
          list.Add(link);
          linkRepo.Insert(link, loadDate);
        }

        var links = linkRepo.GetCurrent().ToList();
        Assert.Equal(insertCount, links.Count());
        Assert.Equal(list, links);


        var timelineRepo = GetTimelineRepo(context);
        var timeline = timelineRepo.GetAll();
        Assert.Equal(insertCount, links.Count());
        foreach (var entry in timeline) {
          Assert.Null(entry.EndDate);
          Assert.Single(links.Where(x => x.PrimaryKey == entry.Reference));
        }
      }
    }

    [Fact]
    public void StoreAndDeleteLink() {
      using (var context = TestDwhDbContext.CreateDWHDbContext()) {
        var keyCache = new BasePrimaryKeyCache();
        var linkRepo = GetLinkRepo(context, keyCache);
        var timelineRepo = GetTimelineRepo(context);

        var loadDate = DateTime.Now;

        var link1 = new L_Link_Timeline();
        for (int i = 1; i <= insertCount; i++) {
          linkRepo.Insert(link1, loadDate.AddDays(i));
          linkRepo.RemoveLink(link1, loadDate.AddDays(i).AddHours(12));

          var timeline = timelineRepo.GetAll().ToList();
        }


        var storedTimeline = timelineRepo.GetAll();
        Assert.Equal(insertCount, storedTimeline.Count());

        var lastDate = loadDate;
        foreach (var entry in storedTimeline) {
          Assert.Equal(OneDay, entry.LoadDate - lastDate);
          Assert.Equal(TwelfeHours, entry.EndDate - entry.LoadDate);
          lastDate = entry.LoadDate;
        }
      }
    }

    [Fact]
    public void StoreLinkList() {
      using (var context = TestDwhDbContext.CreateDWHDbContext()) {
        var keyCache = new BasePrimaryKeyCache();
        var linkRepo = GetLinkRepo(context, keyCache);
        var loadDate = DateTime.Now;

        var list = new List<L_Link_Timeline>();
        for (int i = 0; i < insertCount; i++) {
          var link = new L_Link_Timeline();
          list.Add(link);
        }
        linkRepo.Insert(list, loadDate);

        var links = linkRepo.GetCurrent().ToList();
        Assert.Equal(insertCount, links.Count());
        Assert.Equal(list, links);

        linkRepo.Insert(list, loadDate);

        links = linkRepo.GetCurrent().ToList();
        Assert.Equal(insertCount, links.Count());
        Assert.Equal(list, links);

        var timelineRepo = GetTimelineRepo(context);
        var timeline = timelineRepo.GetAll();
        Assert.Equal(insertCount, links.Count());
        foreach (var entry in timeline) {
          Assert.Null(entry.EndDate);
          Assert.Single(links.Where(x => x.PrimaryKey == entry.Reference));
        }
      }
    }

  }
}
