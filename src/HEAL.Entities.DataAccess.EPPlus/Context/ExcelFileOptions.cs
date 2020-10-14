using System;
using System.IO;

namespace HEAL.Entities.DataAccess.Excel {

  public class ExcelFileOptions : ICloneable, IDisposable {
    public ExcelFileOptions() {
    }

    public ExcelFileOptions(Stream excelFileStream, string worksheetName = default, string filePasword = default) {
      SetFileLocation(excelFileStream, worksheetName, filePasword);
    }

    public void SetFileLocation(Stream excelFileStream, string worksheetName = default, string filePasword = default) {
      ExcelFileStream = excelFileStream;
      WorksheetName = worksheetName ?? DEFAULT_WORKSHEET_NAME;
      FilePassword = filePasword;
    }


    public const int DEFAULT_END_UNLIMITED = 0;
    public const int DEFAULT_HEADER_INDEX = 1;
    public const int DEFAULT_DATA_START_INDEX = 2;
    public const string DEFAULT_WORKSHEET_NAME = "Sheet1";

    /// <summary>
    /// Maximum line number to be considered for parsing
    /// </summary>
    public int DataMaximumLineNumber { get; set; } = DEFAULT_END_UNLIMITED;
    /// <summary>
    /// Line Number of the Header row. Mind that excel is 1-based;
    /// </summary>
    public int HeaderLineNumber { get; set; } = DEFAULT_HEADER_INDEX;
    /// <summary>
    /// Line Number of the first data row. Mind that excel is 1-based;
    /// </summary>
    public int DataStartLineNumber { get; set; } = DEFAULT_DATA_START_INDEX;
    /// <summary>
    /// Name of the worksheet to be parsed. The default is 'DEFAULT'.
    /// </summary>
    public string WorksheetName { get; set; } = DEFAULT_WORKSHEET_NAME;

    /// <summary>
    /// file passport
    /// </summary>
    public string FilePassword { get; set; } = default;

    /// <summary>
    /// stream of the file to be parsed
    /// </summary>
    public Stream ExcelFileStream { get; set; }

    public object Clone() {
      return this.MemberwiseClone();
    }

    public void Dispose() {
      ExcelFileStream?.Dispose();
    }
  }
}
