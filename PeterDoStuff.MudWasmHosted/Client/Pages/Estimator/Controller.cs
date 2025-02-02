using PeterDoStuff.MudWasmHosted.Client.Extensions;
using System.Text;
using static MudBlazor.CategoryTypes;
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
        public static readonly Dictionary<decimal, decimal> ConfidenceIntervalMaps = new()
        {
            { 68, 1 },
            //{ 90, 1.645M },
            { 95, 2 },
            { 99.7M, 3 },
        };

        public static EstimateProject SampleProject()
        {

            return new EstimateProject()
            {
                Name = "Sample Project",
                Groups = [
                    "Implementation",
                    "Testing and Refining",
                    "Analysis and Documentation",
                    "Deliveries"
                ],
                Footnotes = @"**Man-days** from estimation are to determine the monetary value to compensate the service, NOT the schedule that the service would take.

**Calendar-days** are values to determine the estimated completion date from the date the service commenced. The Calendar-days = Man-days x Calendar-days Factor. The Factor depends on the nature of service and the resource the service involved. For example, if the engineer executing the service can only commit 50% of their working days for the service, the Factor would be 200%. If the service involves external stakeholders with unknown effectiveness in consultation, the Factor would be increased.

The Estimate is **NON-TRANSFERABLE**: One individual/team's estimate can't be used to predict how long (in terms of Man-days and Calendar-days) it might take another individual/team to perform the same service.

The Estimate is **TEMPORARY**: An estimate for a service today can be different for the same service next month, as important factors such as requirements and resources can be changed over time.

The Standard Deviation of all tasks is **NOT the Sum of each task's Standard Deviation**. Instead, it is the Sum of each task's (Standard Deviation)^2, then Square-root the Sum. Hence, the Man-days for all tasks is **NOT the Sum of each task's Man-days**.

If the service has lower risk, consider lowering the Contingency's Confidence Level to 68%, corresponding to 1 x SD. Otherwise, consider increasing the Contingency's Confidence Level to 99.7%, corresponding to 3 x SD.

To improve future Estimates, it is recommended to perform Retrospective after all the tasks of the service are completed. Actual man-days should be compared to the estimated man-days, for the whole service or for individual tasks, to fine-tune any estimate that is underestimated or overestimated significantly.",
                Tasks = [
                    new EstimateTask()
                    {
                        GroupIndex = 0,
                        Name = "Create entity tables and columns in Domain",
                        Type = EstimateType.ThreePoint,
                        Best = 0.5M,
                        Likely = 1,
                        Worst = 3
                    },
                    new EstimateTask()
                    {
                        GroupIndex = 0,
                        Name = "Implement automated services",
                        Type = EstimateType.ThreePoint,
                        Best = 1,
                        Likely = 2,
                        Worst = 5
                    },
                    new EstimateTask()
                    {
                        GroupIndex = 0,
                        Name = "Create UI Controls",
                        Type = EstimateType.ThreePoint,
                        Best = 0.5M,
                        Likely = 1,
                        Worst = 3
                    },
                    new EstimateTask()
                    {
                        GroupIndex = 1,
                        Name = "Development Testing",
                        Type = EstimateType.Percentage,
                        Percentage = 10,
                        PercentageByGroupIndex = 0,
                    },
                    new EstimateTask()
                    {
                        GroupIndex = 1,
                        Name = "User Acceptance Feedback Refining",
                        Type = EstimateType.ThreePoint,
                        Best = 1,
                        Likely = 2,
                        Worst = 5,
                    },
                    new EstimateTask()
                    {
                        GroupIndex = 2,
                        Name = "Requirement Analysis and Documentation",
                        Type = EstimateType.Percentage,
                        Percentage = 10,
                        PercentageByGroupIndex = 0,
                    },
                    new EstimateTask()
                    {
                        GroupIndex = 2,
                        Name = "User Acceptance Test Sheet",
                        Type = EstimateType.ThreePoint,
                        Best = 0.5M,
                        Likely = 1,
                        Worst = 3,
                    },
                    new EstimateTask()
                    {
                        GroupIndex = 3,
                        Name = "STG Delivery",
                        Type = EstimateType.Fixed,
                        ExpectedValue = 1,
                        StandardDeviation = 0
                    },
                    new EstimateTask()
                    {
                        GroupIndex = 3,
                        Name = "PRD Delivery",
                        Type = EstimateType.Fixed,
                        ExpectedValue = 0.5M,
                        StandardDeviation = 0
                    },
                ]
            };
        }

        public class EstimateProject
        {
            public string Name { get; set; }
            public List<string> Groups { get; set; } = [];
            public List<EstimateTask> Tasks { get; set; } = [];
            
            public decimal ContingencyConfidence { get; set; } = 95;
            public decimal BreakdownRound { get; set; } = 0.01M;
            public decimal TotalRound { get; set; } = 0.01M;

            public string Footnotes { get; set; }

            public string GenerateMarkdownReport()
            {
                CalculateTasks();
                StringBuilder sb = new StringBuilder();
                decimal sdFactor = ConfidenceIntervalMaps[ContingencyConfidence];
                sb.AppendLine($"| Category | Item | Expected Value (E) | Standard Deviation (SD) | Contingency<br>({ContingencyConfidence}% Confidence - {sdFactor} x SD) | Man-days<br>(E + Contingency) |");
                sb.AppendLine($"| --- | --- | --- | --- | --- | --- |");
                decimal sum = 0;
                foreach (var task in Tasks)
                {
                    var group = Groups[task.GroupIndex];
                    var item = $"{task.Name}<br>`{task.Caption(this)}`";
                    var e = task.ExpectedValue.RoundBy(BreakdownRound);
                    var sd = task.StandardDeviation.RoundBy(BreakdownRound);
                    var contingency = task.Contingency(ContingencyConfidence, BreakdownRound);
                    var (from, to) = task.ConfidenceInterval(ContingencyConfidence, BreakdownRound);
                    sum += to;
                    sb.AppendLine($"| {group} | {item} | {e} | {sd} | {contingency} | {to} |");
                }

                decimal standardDeviation = StandardDeviation;
                var (_, projectTo) = ConfidenceInterval(ContingencyConfidence, TotalRound);
                sb.AppendLine($"| | **TOTAL** | **{ExpectedValue.RoundBy(TotalRound)}** | **{standardDeviation.RoundBy(TotalRound)}** | **{sdFactor * standardDeviation}** | **{projectTo}** |");
                sb.AppendLine();
                sb.AppendLine("- Total Contingency Reduction = " + (sum - projectTo) + " Man-days");
                sb.AppendLine();
                sb.AppendLine("**Footnotes:**");

                foreach (var footnoteParagraph in Footnotes.ToParagraphs())
                    sb.AppendLine($"- {footnoteParagraph}");

                return sb.ToString();
            }

            public void AddGroup()
            {
                var groupName = "Group " + (Groups.Count + 1);
                Groups.Add(groupName);
                AddTask(Groups.Count - 1);
            }

            public void DeleteGroup(int groupIndex)
            {
                DeleteTasksByGroup(groupIndex);
                DecreaseGroupIndexInTasks(groupIndex);
                Groups.RemoveAt(groupIndex);
                CalculateTasks();
            }

            private void DecreaseGroupIndexInTasks(int groupIndex)
            {
                Tasks
                    .Where(t => t.GroupIndex > groupIndex)
                    .ToList()
                    .ForEach(t => t.GroupIndex--);
            }

            private void DeleteTasksByGroup(int groupIndex)
            {
                var from = Tasks.FirstOrDefault(t => t.GroupIndex == groupIndex);
                var to = Tasks.LastOrDefault(t => t.GroupIndex == groupIndex);
                if (from == null || to == null) return;

                var fromIndex = Tasks.IndexOf(from);
                var toIndex = Tasks.IndexOf(to);
                Tasks.RemoveRange(fromIndex, toIndex - fromIndex + 1);
            }

            public void AddTask(int groupIndex)
            {
                var task = new EstimateTask();
                task.GroupIndex = groupIndex;
                var taskCount = Tasks.Count(t => t.GroupIndex == groupIndex);
                task.Name = "Task " + (taskCount + 1);
                Tasks.Add(task);
                Tasks = Tasks.OrderBy(t => t.GroupIndex).ToList();
            }

            public void DeleteTask(EstimateTask task)
            {
                Tasks.Remove(task);
            }

            public decimal ExpectedValue => Tasks.Select(t => t.ExpectedValue.RoundBy(BreakdownRound)).CalculateE();
            public decimal StandardDeviation => Tasks.Select(t => t.StandardDeviation.RoundBy(BreakdownRound)).CalculateSD().RoundBy(BreakdownRound);

            public EstimateProject CalculateTasks()
            {
                foreach (var task in Tasks)
                {
                    if (task.Type == EstimateType.ThreePoint)
                    {
                        task.ExpectedValue = (task.Best + 4 * task.Likely + task.Worst) / 6;
                        task.StandardDeviation = (task.Worst - task.Best) / 6;
                    }
                    if (task.Type == EstimateType.Percentage)
                    {
                        var groupTasks = Tasks.Where(t => t.GroupIndex == task.PercentageByGroupIndex);
                        task.ExpectedValue = groupTasks.Select(t => t.ExpectedValue).CalculateE() * task.Percentage / 100;
                        task.StandardDeviation = groupTasks.Select(t => t.StandardDeviation).CalculateSD() * task.Percentage / 100;
                    }
                }
                return this;
            }

            public (decimal From, decimal To) ConfidenceInterval(decimal confidence, decimal round = 0.01M)
            {
                var e = ExpectedValue.RoundBy(round);
                var contingency = Contingency(confidence, round);
                var from = e - contingency;
                var to = e + contingency;
                return (from, to);
            }

            public decimal Contingency(decimal confidence, decimal round = 0.01M)
            {
                var sdFactor = ConfidenceIntervalMaps[confidence];
                return StandardDeviation.RoundBy(round) * sdFactor;
            }
        }

        public class EstimateTask
        {
            public int GroupIndex { get; set; }
            public string Name { get; set; }
            public EstimateType Type { get; set; }
            public decimal ExpectedValue { get; set; }
            public decimal StandardDeviation { get; set; }
            public decimal Best { get; set; }
            public decimal Likely { get; set; }
            public decimal Worst { get; set; }
            public decimal Percentage { get; set; }
            public int PercentageByGroupIndex { get; set; }

            public (decimal From, decimal To) ConfidenceInterval(decimal confidence, decimal round = 0.01M)
            {
                var e = ExpectedValue.RoundBy(round);
                var contingency = Contingency(confidence, round);
                var from = e - contingency;
                var to = e + contingency;
                return (from, to);
            }

            public decimal Contingency(decimal confidence, decimal round = 0.01M)
            {
                var sdFactor = ConfidenceIntervalMaps[confidence];
                return StandardDeviation.RoundBy(round) * sdFactor;
            }

            public string Caption(EstimateProject project)
            {
                if (Type == EstimateType.Fixed)
                    return "Fixed";

                if (Type == EstimateType.ThreePoint)
                    return $"Best: {Best} - Likely: {Likely} - Worst: {Worst}";

                if (Type == EstimateType.Percentage)
                    return $"{Percentage}% of [{project.Groups[PercentageByGroupIndex]}]";

                throw new NotImplementedException();
            }
        }

        public enum EstimateType
        {
            ThreePoint, Percentage, Fixed
        }
    }
}
