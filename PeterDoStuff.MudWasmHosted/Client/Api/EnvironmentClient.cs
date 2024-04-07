using PeterDoStuff.MudWasmHosted.Shared;

namespace PeterDoStuff.MudWasmHosted.Client.Api
{
    public class EnvironmentClient : ApiClient, EnvironmentApi
    {
        protected override string Route => "api/environment";

        public Task<string> Get(string key)
            => SendToApiAsGet<string>("", ("key", key));

        public Task<string> GetMyFirstEnvironment()
            => SendToApiAsGet<string>("GetMyFirstEnvironment");
    }
}
