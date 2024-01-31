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
        public static void WriteToConsole(this string @this)
            => Console.WriteLine(@this);
    }
}
