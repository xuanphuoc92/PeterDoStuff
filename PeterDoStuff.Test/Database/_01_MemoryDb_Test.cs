using PeterDoStuff.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeterDoStuff.Test.Database
{
    [TestClass]
    public class _01_MemoryDb_Test : _00_BaseDb_Test
    {
        protected override BaseDb GetDb()
        {
            return new MemoryDb();
        }
    }
}
