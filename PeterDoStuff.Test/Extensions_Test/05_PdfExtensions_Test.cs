using PeterDoStuff.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeterDoStuff.Test.Extensions_Test
{
    [TestClass]
    public class _05_PdfExtensions_Test
    {
        [TestMethod]
        public void _01_ConvertUriToPdfFile()
        {
            string uri = "https://www.google.com";
            uri.ConvertUriToPdf("test.pdf");
        }

        [TestMethod]
        public void _02_ConvertHtmlFileToPdfFile()
        {
            string htmlFilePath = "Extensions_Test\\test.html";
            htmlFilePath.ConvertFileToPdf("test.pdf");
        }
    }
}
