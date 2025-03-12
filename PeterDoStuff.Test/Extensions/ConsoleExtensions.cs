using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeterDoStuff.Test.Extensions
{
    public static class ConsoleExtensions
    {
        /// <summary>
        /// Write to Console
        /// </summary>
        /// <param name="this"></param>
        public static void WriteToConsole(this string @this, string tag = "")
            => Console.WriteLine((tag == "" ? "" : $"[{tag}] ") + @this);

        /// <summary>
        /// Write Exception to Console
        /// </summary>
        /// <param name="this"></param>
        public static void WriteToConsole(this Exception @this)
        {
            Exception? ex = @this;
            while (ex != null)
            {
                Console.WriteLine(ex.Message + '\n' + ex.StackTrace);
                ex = ex.InnerException;
            }
        }
    }
}
