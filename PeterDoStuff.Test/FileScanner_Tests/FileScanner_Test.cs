using ApprovalTests.Reporters;
using FluentAssertions;
using PeterDoStuff.Test.Extensions;
using PeterDoStuff.Tools;

namespace PeterDoStuff.Test.FileScanner_Tests
{
    [TestClass]
    [UseReporter(typeof(DiffReporter))]
    public class FileScanner_Test
    {
        [TestMethod]
        public void _01_ScanZipFile()
        {
            using var scanner = new FileScanner();
            var result = scanner.ScanZip("FileScanner_Tests\\TestZipFile.zip");
            result.Should().Be(FileScanner.SUCCESSFUL);
            scanner.ZipStats.Count.Should().Be(4);
            string csvContent = scanner.ToCsv();
            csvContent.Verify();
        }

        [TestMethod]
        public void _02_ScanNonExistFile()
        {
            using var scanner = new FileScanner();
            var result = scanner.ScanZip("NonExistFile.zip");
            result.Should().NotBe(FileScanner.SUCCESSFUL);
            result.WriteToConsole();
        }

        [TestMethod]
        public void _03_ScanNonZipFile()
        {
            using var scanner = new FileScanner();
            var result = scanner.ScanZip("FileScanner_Tests\\TestTextFile.txt");
            result.Should().NotBe(FileScanner.SUCCESSFUL);
            result.WriteToConsole();
        }
    }
}
