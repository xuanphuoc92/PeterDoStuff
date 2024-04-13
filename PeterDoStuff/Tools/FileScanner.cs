using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace PeterDoStuff.Tools
{
    public class FileScanner : IDisposable
    {
        public void Dispose()
        {
        }

        public List<FileScannerZipStat> ZipStats
        { get; private set; } = new List<FileScannerZipStat>();

        public string ScanZip(string path)
        {
            try
            {
                using FileStream fileStream = File.OpenRead(path);
                return ScanZip(fileStream);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public const string SUCCESSFUL = "Scan is successful";

        public string ScanZip(Stream stream)
        {
            try
            {
                using ZipArchive archive = new ZipArchive(stream);
                ZipStats.Clear();
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    var stat = new FileScannerZipStat()
                    {
                        FilePath = entry.FullName,
                        Extension = GetExtension(entry.Name),
                        NumberOfLines = GetNumberOfLines(entry.Open())
                    };
                    ZipStats.Add(stat);
                }
                return SUCCESSFUL;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private string GetExtension(string name)
        {
            int lastDotIndex = name.IndexOf('.');
            if (lastDotIndex == -1) return name;
            return name.Substring(lastDotIndex);
        }

        private int GetNumberOfLines(Stream stream)
        {
            using StreamReader sr = new StreamReader(stream, Encoding.UTF8);
            int lineCount = 0;
            while (sr.ReadLine() != null) lineCount++;
            return lineCount;

            //// Show 1 if the file is empty or not a text file
            //return lineCount == 0 ? 1 : lineCount;
        }

        public string ToCsv()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("File Path,Extension,Number Of Lines");
            ZipStats.ForEach(s => sb.AppendLine($"{s.FilePath},{s.Extension},{s.NumberOfLines}"));
            return sb.ToString();
        }
    }

    public class FileScannerZipStat
    {
        public string FilePath { get; set; }
        public string Extension { get; set; }
        public int NumberOfLines { get; set; }
    }
}
