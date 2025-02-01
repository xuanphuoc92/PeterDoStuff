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

        public static List<ViewRow> ToViewRows(this Project project)
        {
            List<ViewRow> results = [];

            results.Add(new ViewRow { Project = project });
            foreach (var group in project.Groups)
            {
                results.Add(new ViewRow { Group = group });
                foreach (var task in group.Tasks)
                {
                    results.Add(new ViewRow { Task = task });
                }
            }
            return results;
        }
    }

    public class Controller
    {
        public class ViewRow
        {
            public Project? Project { get; set; } = null;
            public Group? Group { get; set; } = null;
            public EstimateTask? Task { get; set; } = null;

            private EstimateValue? EValue => (EstimateValue)Project ?? (EstimateValue)Group ?? Task;

            public decimal ExpectedValue => EValue?.ExpectedValue.RoundBy(0.01M) ?? 0;
            public decimal StandardDeviation => EValue?.StandardDeviation.RoundBy(0.01M) ?? 0;

            public string ConfidenceInterval(decimal confidenceInteval)
            {
                var sdFactor = ConfidenceIntervalMaps[confidenceInteval];
                var e = ExpectedValue;
                var sd = StandardDeviation;
                var from = e - sdFactor * sd;
                var to = e + sdFactor * sd;
                return $"{from.RoundBy(0.01M)} - {to.RoundBy(0.01M)}";
            }
        }

        public static readonly Dictionary<decimal, decimal> ConfidenceIntervalMaps = new()
        {
            { 68, 1 },
            { 90, 1.645M },
            { 95, 2 },
            { 99.7M, 3 },
        };


        public static Project SampleProject()
        {
            return new Project()
            {
                Groups = [
                    new Group()
                    {
                        Name = "Implementation",
                        Tasks = [
                            new EstimateTask()
                            {
                                Description = "Create entity tables and columns in Domain",
                                EstimateType = EstimateType.ThreePoint,
                                ThreePointEstimate = new ThreePointEstimate()
                                {
                                    Best = 0.5M,
                                    Likely = 1,
                                    Worst = 3
                                }
                            },
                            new EstimateTask()
                            {
                                Description = "Implement functional services",
                                EstimateType = EstimateType.ThreePoint,
                                ThreePointEstimate = new ThreePointEstimate()
                                {
                                    Best = 1,
                                    Likely = 2,
                                    Worst = 5
                                }
                            },
                            new EstimateTask()
                            {
                                Description = "Create UI controls",
                                EstimateType = EstimateType.ThreePoint,
                                ThreePointEstimate = new ThreePointEstimate()
                                {
                                    Best = 0.5M,
                                    Likely = 1,
                                    Worst = 3
                                }
                            },
                        ]
                    },
                    new Group()
                    {
                        Name = "Testing and Refining",
                        Tasks = [
                            new EstimateTask()
                            {
                                Description = "Development Testing",
                                EstimateType = EstimateType.Percentage,
                                PercentageEstimate = new PercentageEstimate()
                                {
                                    Percentage = 10,
                                    GroupIndex = 0,
                                }
                            },
                            new EstimateTask()
                            {
                                Description = "User Acceptance Feedback Refining",
                                EstimateType = EstimateType.ThreePoint,
                                ThreePointEstimate = new ThreePointEstimate()
                                {
                                    Best = 1,
                                    Likely = 2,
                                    Worst = 5
                                }
                            },
                        ]
                    },
                    new Group()
                    {
                        Name = "Analysis and Documentation",
                        Tasks = [
                            new EstimateTask()
                            {
                                Description = "Requirement Analysis and Documentation",
                                EstimateType = EstimateType.Percentage,
                                PercentageEstimate = new PercentageEstimate()
                                {
                                    Percentage = 10,
                                    GroupIndex = 0,
                                }
                            },
                            new EstimateTask()
                            {
                                Description = "User Acceptance Test Sheet",
                                EstimateType = EstimateType.ThreePoint,
                                ThreePointEstimate = new ThreePointEstimate()
                                {
                                    Best = 0.5M,
                                    Likely = 1,
                                    Worst = 3
                                }
                            },
                        ]
                    },
                    new Group()
                    {
                        Name = "Deployment",
                        Tasks = [
                            new EstimateTask()
                            {
                                Description = "UAT Deployment",
                                EstimateType = EstimateType.Fixed,
                                FixedEstimate = new FixedEstimate()
                                {
                                    Value = 1M,
                                    Error = 0M
                                }
                            },
                            new EstimateTask()
                            {
                                Description = "PRD Deployment",
                                EstimateType = EstimateType.Fixed,
                                FixedEstimate = new FixedEstimate()
                                {
                                    Value = 0.5M,
                                    Error = 0M
                                }
                            },
                        ]
                    }
                ]
            };
        }

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
                ExpectedValue = Groups.Select(g => g.ExpectedValue).CalculateE();
                StandardDeviation = Groups.Select(g => g.StandardDeviation).CalculateSD();
            }
        }

        public class Group: EstimateValue
        {
            public Group() { }

            public Group(int number) { Name = "Group " + number; }

            public string Name { get; set; }

            public List<EstimateTask> Tasks { get; set; } = [];

            public override void Calculate(Project project)
            {
                Tasks.ForEach(g => g.Calculate(project));
                ExpectedValue = Tasks.Select(g => g.ExpectedValue).CalculateE();
                StandardDeviation = Tasks.Select(g => g.StandardDeviation).CalculateSD();
            }
        }

        public class EstimateTask : EstimateValue
        {
            public EstimateTask() { }

            public EstimateTask(int number) { Description = "Task " + number; }

            public string Description { get; set; }

            public EstimateType EstimateType { get; set; } = EstimateType.Fixed;

            public FixedEstimate FixedEstimate { get; set; } = new();
            public ThreePointEstimate ThreePointEstimate { get; set; } = new();
            public PercentageEstimate PercentageEstimate { get; set; } = new();

            public string Caption(Project project)
            {
                if (EstimateType == EstimateType.Fixed)
                    return "Fixed";
                    
                if (EstimateType == EstimateType.ThreePoint)
                    return $"Best: {ThreePointEstimate.Best} | Likely: {ThreePointEstimate.Likely} | Worst: {ThreePointEstimate.Worst}";

                if (EstimateType == EstimateType.Percentage)
                    return $"{PercentageEstimate.Percentage}% of [{project.Groups[PercentageEstimate.GroupIndex].Name}]";

                throw new NotImplementedException();
            }

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
                ExpectedValue = Value;
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
                ExpectedValue = (Best + 4 * Likely + Worst) / 6;
                //ExpectedValue = ExpectedValue.RoundBy(project.RoundStep);
                StandardDeviation = (Worst - Best) / 6;
                //StandardDeviation = StandardDeviation.RoundBy(project.RoundStep);
            }
        }

        public class PercentageEstimate : EstimateValue
        {
            public decimal Percentage { get; set; }
            public int GroupIndex { get; set; } = 0;

            public override void Calculate(Project project)
            {
                var group = project.Groups[GroupIndex];
                ExpectedValue = group.ExpectedValue * Percentage / 100;
                //ExpectedValue = ExpectedValue.RoundBy(project.RoundStep);
                StandardDeviation = group.StandardDeviation * Percentage / 100;
                //StandardDeviation = StandardDeviation.RoundBy(project.RoundStep);
            }
        }
    }
}
