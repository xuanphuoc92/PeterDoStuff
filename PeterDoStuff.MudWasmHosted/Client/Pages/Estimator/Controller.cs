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

        public static decimal RoundBy(this decimal number, decimal step)
        {
            if (step <= 0)
                throw new ArgumentException("Step must be greater than zero.", nameof(step));
            return Math.Round(number / step) * step;
        }
    }

    public class Controller
    {
        public abstract class EstimateValue
        {
            public decimal ExpectedValue { get; set; }
            public decimal StandardDeviation { get; set; }
            public abstract void Calculate(Project project);
        }

        public class Project : EstimateValue
        {
            public string Name { get; set; } = "My Project";

            public decimal RoundStep { get; set; } = 0.5M;

            public List<Group> Groups { get; set; } = [];

            public override void Calculate(Project project)
            {
                Groups.ForEach(g => g.Calculate(project));
                ExpectedValue = Groups.Select(g => g.ExpectedValue).CalculateE().RoundBy(project.RoundStep);
                StandardDeviation = Groups.Select(g => g.StandardDeviation).CalculateSD().RoundBy(project.RoundStep);
            }
        }

        public class Group(int number) : EstimateValue
        {
            public string Name { get; set; } = "Group " + number;

            public List<EstimateTask> Tasks { get; set; } = [];

            public override void Calculate(Project project)
            {
                Tasks.ForEach(g => g.Calculate(project));
                ExpectedValue = Tasks.Select(g => g.ExpectedValue).CalculateE().RoundBy(project.RoundStep);
                StandardDeviation = Tasks.Select(g => g.StandardDeviation).CalculateSD().RoundBy(project.RoundStep);
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
                ExpectedValue = value.ExpectedValue;
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
                ExpectedValue = Value.RoundBy(project.RoundStep);
                StandardDeviation = Error.RoundBy(project.RoundStep);
            }
        }

        public class ThreePointEstimate : EstimateValue
        {
            public decimal Best { get; set; }
            public decimal Likely { get; set; }
            public decimal Worst { get; set; }

            public override void Calculate(Project project)
            {
                ExpectedValue = (Best + 4 * Likely + Worst) / 6;
                ExpectedValue = ExpectedValue.RoundBy(project.RoundStep);
                StandardDeviation = (Worst - Best) / 6;
                StandardDeviation = StandardDeviation.RoundBy(project.RoundStep);
            }
        }

        public class PercentageEstimate : EstimateValue
        {
            public decimal Percentage { get; set; }
            public int GroupIndex { get; set; } = 0;

            public override void Calculate(Project project)
            {
                var group = project.Groups[GroupIndex];
                ExpectedValue = group.ExpectedValue * Percentage;
                ExpectedValue = ExpectedValue.RoundBy(project.RoundStep);
                StandardDeviation = group.StandardDeviation * Percentage;
                StandardDeviation = StandardDeviation.RoundBy(project.RoundStep);
            }
        }
    }
}
