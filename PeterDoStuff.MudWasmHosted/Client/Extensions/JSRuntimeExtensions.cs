using Microsoft.JSInterop;
using PeterDoStuff.Extensions;
using System.Text;

namespace PeterDoStuff.MudWasmHosted.Client.Extensions
{
    public static class JSRuntimeExtensions
    {
        public static async Task DownloadJson(this IJSRuntime JS, object obj, string fileName, string defaultFileName = "Default")
        {
            if (fileName.IsNullOrEmpty())
                fileName = defaultFileName;
            
            if (fileName.ToLowerInvariant().EndsWith(".json") == false)
                fileName += ".json";

            await JS.DownloadTextFile(fileName, obj.ToJson(beautify: true));
        }

        public static async Task DownloadTextFile(this IJSRuntime JS, string fileName, string fileContent)
        {
            using MemoryStream fileStream = new MemoryStream(
                Encoding.UTF8.GetBytes(fileContent));
            using var streamRef = new DotNetStreamReference(stream: fileStream);
            await JS.InvokeVoidAsync("downloadFileFromStream", fileName.Trim(), streamRef);
        }
    }
}
