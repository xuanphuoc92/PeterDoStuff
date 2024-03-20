using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PeterDoStuff.Extensions;
using PeterDoStuff.Test.Extensions;

namespace PeterDoStuff.Test.Extensions_Test
{
    [TestClass]
    public class _04_QRCodeExtensions_Test
    {
        [TestMethod]
        public void _01_GenerateQRCode()
        {
            string input = "https://example.com";
            var path = "testQrCode.png";
            File.WriteAllBytes(path, input.ToQRCode());
            $"Access the bin folder and find file {path} to scan the QR code and verify if it is {input}".WriteToConsole();
        }
    }
}
