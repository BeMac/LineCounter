using System.Collections.Generic;
using System.IO;

namespace LineSampleBrianMc.Models
{
    public class FileInfoModel
    {
        public int IncludedLineCount { get; set; }
        public int ExludedLineCount { get; set; }
        public IEnumerable<FileInfo> IncludedFiles { get; set; }
        public IEnumerable<FileInfo> ExcludedFiles { get; set; }
    }
}