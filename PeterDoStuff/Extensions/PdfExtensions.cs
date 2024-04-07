using ChromiumHtmlToPdfLib;
using ChromiumHtmlToPdfLib.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeterDoStuff.Extensions
{
    public static class PdfExtensions
    {
        public static void ConvertUriToPdf(this string uri, string pdfFilePath)
        {
            var pageSettings = new PageSettings();
            using var converter = new Converter("C:\\Program Files (x86)\\Microsoft\\Edge\\Application\\msedge.exe");
            converter.UseOldHeadlessMode = true;
            converter.ConvertToPdf(new ConvertUri(uri), pdfFilePath, pageSettings);
        }

        public static void ConvertFileToPdf(this string htmlFilePath, string pdfFilePath)
        {
            var pageSettings = new PageSettings();
            using var converter = new Converter("C:\\Program Files (x86)\\Microsoft\\Edge\\Application\\msedge.exe");
            converter.UseOldHeadlessMode = true;
            string html = File.ReadAllText(htmlFilePath);
            converter.ConvertToPdf(html, pdfFilePath, pageSettings);
        }
    }
}
