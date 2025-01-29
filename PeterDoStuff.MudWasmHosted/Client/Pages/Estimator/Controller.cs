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
            public ThreePointEstimate ThreePointEstimate { get; set; } = new();
            public PercentageEstimate PercentageEstimate { get; set; } = new();
        }

        public enum EstimateType 
        {
            Fixed, ThreePoint, Percentage
        }

        public class FixedEstimate
        {
            public decimal Value { get; set; }
            public decimal Error { get; set; }
        }

        public class ThreePointEstimate
        {
            public decimal Best { get; set; }
            public decimal Likely { get; set; }
            public decimal Worst { get; set; }
        }

        public class PercentageEstimate
        {
            public decimal Percentage { get; set; }
            public int GroupIndex { get; set; } = -1;
        }
    }
}
