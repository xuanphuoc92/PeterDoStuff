using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PeterDoStuff.Tools;

namespace PeterDoStuff.MudWasmHosted.Shared
{
    public interface DatabaseApi
    {
        Task<DbAccess> Access(string accessKey);
        Task<QueryOutput> Post(string sql);
    }

    public record DbAccess(bool IsSuccess, string Warning, string Token);
}
