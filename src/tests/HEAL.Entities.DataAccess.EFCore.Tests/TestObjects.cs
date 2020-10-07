using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using HEAL.Entities.Objects;
using HEAL.Entities.Objects.Dwh.DataVaultV2;
using HEAL.Entities.Utils.Enumerables;

namespace HEAL.Entities.DataAccess.EFCore.Tests {

  public class LoadInformation : IDomainObject<long>, IReferenceLoadInformation, IStoreLoadInformation {
    public LoadInformation() {

    }
    public LoadInformation(string recordSource, string loadedBy) {
      RecordSource = recordSource;
      LoadedBy = loadedBy;
    }


    public string RecordSource { get; set; }
    public string LoadedBy { get; set; }
    public long LoadReference { get { return PrimaryKey; } set { PrimaryKey = value; } }

    public long PrimaryKey { get; set; }

    public string GetBusinessKeyString() {
      return $"{LoadedBy}{RecordSource}";
    }

    public int CompareTo(IDomainObject<string> other) {
      return this.PrimaryKey.CompareTo(other.PrimaryKey);
    }

    public int CompareTo(IDomainObject<long> other) {
      throw new NotImplementedException();
    }
  }

  #region DEFAULT
  public class H_TestHub : IHub {
    public H_TestHub() {

    }
    public H_TestHub(string testNr) {
      TestNr = testNr;
    }
    public string TestNr { get; set; }

    public DateTime LoadDate { get; set; }
    public string PrimaryKey { get; set; }

    public int CompareTo(IDomainObject<string> other) {
      return this.PrimaryKey.CompareTo(other.PrimaryKey);
    }

    public string GetBusinessKeyString() {
      return TestNr;
    }
  }
  public class L_TestLink : ILink {
    public L_TestLink() {

    }
    public L_TestLink(string first, string second) {
      FirstTest = first;
      SecondTest = second;
    }

    public string FirstTest { get; set; }
    public string SecondTest { get; set; }

    public string PrimaryKey { get; set; }
    public DateTime LoadDate { get; set; }

    public string GetBusinessKeyString() {
      return $"{FirstTest}{SecondTest}";
    }
    public int CompareTo(IDomainObject<string> other) {
      return this.PrimaryKey.CompareTo(other.PrimaryKey);
    }
  }


  public class S_TestSatellite : ISatellite {
    public S_TestSatellite() {

    }
    public S_TestSatellite(int value, string testNr) {
      Value = value;
      TestNr = testNr;
    }
    public int Value { get; set; }
    public string TestNr { get; set; } //BusinessKey

    public DateTime LoadDate { get; set; }

    public ISatellitePrimaryKey<string> PrimaryKey
    {
      get { return this.GetSatellitePrimaryKey(); }
      set { this.SetSatellitePrimaryKey(value); }
    }
    public string Reference { get; set; }
    public string GetBusinessKeyString() {
      return TestNr;
    }
    public int CompareTo(IDomainObject<ISatellitePrimaryKey<string>> other) {
      return this.PrimaryKey.CompareTo(other.PrimaryKey);
    }
  }

  #endregion

  #region Default Objects
  public class H_TestHub_Default : H_TestHub {
    public H_TestHub_Default() : base() {

    }
    public H_TestHub_Default(string testNr) : base(testNr) {
    }
  }

  public class L_TestLink_Default : L_TestLink {
    public L_TestLink_Default() : base() {

    }
    public L_TestLink_Default(string first, string second) : base(first, second) {
    }
  }

  public class S_TestSatellite_Default : S_TestSatellite {
    public S_TestSatellite_Default() : base() {

    }
    public S_TestSatellite_Default(int value, string testNr) : base(value, testNr) {
    }
  }
  #endregion 

  #region Stores Audit Attributes

  public class H_TestHub_Stores : H_TestHub, IStoreLoadInformation {
    public H_TestHub_Stores() : base() {

    }
    public H_TestHub_Stores(string testNr) : base(testNr) {
    }
    public H_TestHub_Stores(string testNr, string recordSource, string loadedBy) : base(testNr) {
      RecordSource = recordSource;
      LoadedBy = loadedBy;
    }

    public string RecordSource { get; set; }
    public string LoadedBy { get; set; }
  }

  public class L_TestLink_Stores : L_TestLink, IStoreLoadInformation {
    public L_TestLink_Stores() : base() {

    }
    public L_TestLink_Stores(string first, string second) : base(first, second) {
    }
    public L_TestLink_Stores(string first, string second, string recordSource, string loadedBy) : base(first, second) {
      RecordSource = recordSource;
      LoadedBy = loadedBy;
    }
    public string RecordSource { get; set; }
    public string LoadedBy { get; set; }
  }

  public class S_TestSatellite_Stores : S_TestSatellite, IStoreLoadInformation {
    public S_TestSatellite_Stores() : base() {

    }
    public S_TestSatellite_Stores(int value, string testNr) : base(value, testNr) {
    }
    public S_TestSatellite_Stores(int value, string testNr, string recordSource, string loadedBy) : base(value, testNr) {
      RecordSource = recordSource;
      LoadedBy = loadedBy;
    }

    public string RecordSource { get; set; }
    public string LoadedBy { get; set; }
  }

  #endregion

  #region References Audit Information
  public class H_TestHub_References : H_TestHub, IReferenceLoadInformation {
    public H_TestHub_References() : base() {

    }
    public H_TestHub_References(string testNr) : base(testNr) {
    }
    public H_TestHub_References(string testNr, long loadReference) : base(testNr) {
      LoadReference = loadReference;
    }
    public long LoadReference { get; set; }
  }

  public class L_TestLink_References : L_TestLink, IReferenceLoadInformation {
    public L_TestLink_References() : base() {

    }
    public L_TestLink_References(string first, string second, long loadReference) : base(first, second) {
      LoadReference = loadReference;
    }
    public L_TestLink_References(string first, string second) : base(first, second) {
    }
    public long LoadReference { get; set; }
  }

  public class S_TestSatellite_References : S_TestSatellite, IReferenceLoadInformation {
    public S_TestSatellite_References() : base() {

    }
    public S_TestSatellite_References(int value, string testNr) : base(value, testNr) {
    }
    public S_TestSatellite_References(int value, string testNr, long loadReference) : base(value, testNr) {
      LoadReference = loadReference;
    }
    public long LoadReference { get; set; }
  }
  #endregion


  #region Deleteable Link

  public class L_Link_Timeline : ILinkTimeline<string,S_LinkTimelineEntry> {
    public L_Link_Timeline() {
      Key = Guid.NewGuid().ToString();
    }
    public string Key { get; set; }

    public string PrimaryKey { get; set; }
    public DateTime LoadDate { get; set; }
    public ICollection<S_LinkTimelineEntry> Timeline { get; set; }

    public S_LinkTimelineEntry MostRecentEntry => Timeline.MaxOrDefaultBy(x => x.LoadDate);

    public int CompareTo(IDomainObject<string> other) {
      return this.PrimaryKey.CompareTo(other.PrimaryKey);
    }

    public string GetBusinessKeyString() {
      return Key;
    }
  }

  public class S_LinkTimelineEntry : ITimelineSatellite<string> {
    public L_Link_Timeline Link { get; set; }

    public string Key { get; set; }

    public string Reference { get; set; }
    public DateTime LoadDate { get; set; }
    public DateTime? EndDate { get; set; }

    public ISatellitePrimaryKey<string> PrimaryKey {
      get { return this.GetSatellitePrimaryKey<S_LinkTimelineEntry,string>(); }
      set { this.SetSatellitePrimaryKey(value); }
    }
    public string GetBusinessKeyString() {
      return Key;
    }
    public int CompareTo(IDomainObject<ISatellitePrimaryKey<string>> other) {
      return this.PrimaryKey.CompareTo(other.PrimaryKey);
    }
  }

  #endregion
}
