using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeterDoStuff.MudWasmHosted.Shared
{
    public class DbAccess
    {
        public bool IsSuccess { get; set; }
        public string Warning { get; set; }
        public string Token { get; set; }
    }
}
