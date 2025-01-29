using System.Threading.Tasks;
using static PeterDoStuff.MudWasmHosted.Client.Pages.Estimator.Controller;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.Estimator
{
    internal static class EstimatorExtensions
    {
    }

    public class Controller
    {
        public class Project
        {
            public string Name { get; set; } = "My Project";
            public List<Group> Groups { get; set; } = [];
        }

        public class Group(int number)
        {
            public string Name { get; set; } = "Group " + number;
        }
    }
}
