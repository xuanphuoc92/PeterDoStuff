using System.Threading.Tasks;
using static PeterDoStuff.MudWasmHosted.Client.Pages.Estimator.Controller;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.Estimator
{
    internal static class EstimatorExtensions
    {
        public static decimal CalculateE(this IEnumerable<decimal> Es)
            => Es.Sum();

        public static decimal CalculateSD(this IEnumerable<decimal> SDs)
            => (decimal)Math.Sqrt((double)SDs.Sum(sd => sd * sd));
    }

    public class Controller
    {
        public abstract class EstimateValue
        {
            public decimal Expectancy { get; set; }
            public decimal StandardDeviation { get; set; }
            public abstract void Calculate(Project project);
        }

        public class Project : EstimateValue
        {
            public string Name { get; set; } = "My Project";
            public List<Group> Groups { get; set; } = [];

            public override void Calculate(Project project)
            {
                Groups.ForEach(g => g.Calculate(project));
                Expectancy = Groups.Select(g => g.Expectancy).CalculateE();
                StandardDeviation = Groups.Select(g => g.StandardDeviation).CalculateSD();
            }
        }

        public class Group(int number) : EstimateValue
        {
            public string Name { get; set; } = "Group " + number;

            public List<EstimateTask> Tasks { get; set; } = [];

            public override void Calculate(Project project)
            {
                Tasks.ForEach(g => g.Calculate(project));
                Expectancy = Tasks.Select(g => g.Expectancy).CalculateE();
                StandardDeviation = Tasks.Select(g => g.StandardDeviation).CalculateSD();
            }
        }

        public class EstimateTask(int number) : EstimateValue
        {
            public string Description { get; set; } = "Task " + number;

            public EstimateType EstimateType { get; set; } = EstimateType.Fixed;

            public FixedEstimate FixedEstimate { get; set; } = new();
            public ThreePointEstimate ThreePointEstimate { get; set; } = new();
            public PercentageEstimate PercentageEstimate { get; set; } = new();

            public override void Calculate(Project project)
            {
                EstimateValue value = EstimateType switch
                {
                    EstimateType.Fixed => FixedEstimate,
                    EstimateType.ThreePoint => ThreePointEstimate,
                    EstimateType.Percentage => PercentageEstimate,
                    _ => throw new NotImplementedException()
                };

                value.Calculate(project);
                Expectancy = value.Expectancy;
                StandardDeviation = value.StandardDeviation;
            }
        }

        public enum EstimateType 
        {
            Fixed, ThreePoint, Percentage
        }

        public class FixedEstimate : EstimateValue
        {
            public decimal Value { get; set; }
            public decimal Error { get; set; }

            public override void Calculate(Project project)
            {
                Expectancy = Value;
                StandardDeviation = Error;
            }
        }

        public class ThreePointEstimate : EstimateValue
        {
            public decimal Best { get; set; }
            public decimal Likely { get; set; }
            public decimal Worst { get; set; }

            public override void Calculate(Project project)
            {
                Expectancy = (Best + 4 * Likely + Worst) / 6;
                StandardDeviation = (Worst - Best) / 6;
            }
        }

        public class PercentageEstimate : EstimateValue
        {
            public decimal Percentage { get; set; }
            public int GroupIndex { get; set; } = 0;

            public override void Calculate(Project project)
            {
                var group = project.Groups[GroupIndex];
                Expectancy = group.Expectancy * Percentage;
                StandardDeviation = group.StandardDeviation * Percentage;
            }
        }
    }
}
