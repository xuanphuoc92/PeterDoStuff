using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeterDoStuff.MudWasmHosted.Shared
{
    public interface EnvironmentApi
    {
        Task<string> Get(string key);
        Task<string> GetMyFirstEnvironment();
    }
}
