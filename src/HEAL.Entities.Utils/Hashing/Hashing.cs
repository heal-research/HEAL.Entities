using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace HEAL.Entities.Utils.Hashing
{
  /// <summary>
  /// Provides static methods for cryptographic hash functions for e.g. Data Vault primary key hashing
  /// </summary>
  public static class HashFunctions {
    public static string MD5(string bKey) {
      // step 1, calculate MD5 hash from input
      MD5 md5 = System.Security.Cryptography.MD5.Create();
      return BitConverter.ToString(
                  md5.ComputeHash(
                        System.Text.Encoding.ASCII.GetBytes(bKey)))
             .Replace("-", "");
    }
  }
}
