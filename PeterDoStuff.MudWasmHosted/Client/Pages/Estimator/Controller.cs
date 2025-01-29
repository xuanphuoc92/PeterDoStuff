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

            public List<EstimateTask> Tasks { get; set; } = [];
        }

        public class EstimateTask(int number)
        {
            public string Description { get; set; } = "Task " + number;

            public EstimateType EstimateType { get; set; } = EstimateType.Fixed;

            public FixedEstimate FixedEstimate { get; set; } = new();
        }

        public enum EstimateType 
        {
            Fixed, ThreePoint, PercentageReference
        }

        public class FixedEstimate
        {
            public decimal Value { get; set; }
            public decimal Error { get; set; }
        }
    }
}
