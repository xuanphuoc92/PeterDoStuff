using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PeterDoStuff.Test.FileScanner_Tests
{
    [TestClass]
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
            Console.WriteLine(csvContent);
        }

        [TestMethod]
        public void _02_ScanNonExistFile()
        {
            using var scanner = new FileScanner();
            var result = scanner.ScanZip("NonExistFile.zip");
            result.Should().NotBe(FileScanner.SUCCESSFUL);
            Console.WriteLine(result);
        }

        [TestMethod]
        public void _03_ScanNonZipFile()
        {
            using var scanner = new FileScanner();
            var result = scanner.ScanZip("FileScanner_Tests\\TestTextFile.txt");
            result.Should().NotBe(FileScanner.SUCCESSFUL);
            Console.WriteLine(result);
        }
    }
}
