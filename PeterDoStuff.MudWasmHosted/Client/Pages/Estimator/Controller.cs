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

        public static NewProject SampleNewProject()
        {

            return new NewProject()
            {
                Groups = [
                    "Implementation",
                    "Testing and Refining",
                    "Analysis and Documentation",
                    "Deliveries"
                ],
                Tasks = [
                    new NewTask()
                    {
                        GroupIndex = 0,
                        Name = "Create entity tables and columns in Domain",
                        Type = EstimateType.ThreePoint,
                        Best = 0.5M,
                        Likely = 1,
                        Worst = 3
                    },
                    new NewTask()
                    {
                        GroupIndex = 0,
                        Name = "Implement automated services",
                        Type = EstimateType.ThreePoint,
                        Best = 1,
                        Likely = 2,
                        Worst = 5
                    },
                    new NewTask()
                    {
                        GroupIndex = 0,
                        Name = "Create UI Controls",
                        Type = EstimateType.ThreePoint,
                        Best = 0.5M,
                        Likely = 1,
                        Worst = 3
                    },
                    new NewTask()
                    {
                        GroupIndex = 1,
                        Name = "Development Testing",
                        Type = EstimateType.Percentage,
                        Percentage = 10,
                        PercentageByGroupIndex = 0,
                    },
                    new NewTask()
                    {
                        GroupIndex = 1,
                        Name = "User Acceptance Feedback Refining",
                        Type = EstimateType.ThreePoint,
                        Best = 1,
                        Likely = 2,
                        Worst = 5,
                    },
                    new NewTask()
                    {
                        GroupIndex = 2,
                        Name = "Requirement Analysis and Documentation",
                        Type = EstimateType.Percentage,
                        Percentage = 10,
                        PercentageByGroupIndex = 0,
                    },
                    new NewTask()
                    {
                        GroupIndex = 2,
                        Name = "User Acceptance Test Sheet",
                        Type = EstimateType.ThreePoint,
                        Best = 0.5M,
                        Likely = 1,
                        Worst = 3,
                    },
                    new NewTask()
                    {
                        GroupIndex = 3,
                        Name = "STG Delivery",
                        Type = EstimateType.Fixed,
                        ExpectedValue = 1,
                        StandardDeviation = 0
                    },
                    new NewTask()
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

        public class NewProject
        {
            public string Name { get; set; }
            public List<string> Groups { get; set; } = [];
            public List<NewTask> Tasks { get; set; } = [];

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
                var task = new NewTask();
                task.GroupIndex = groupIndex;
                var taskCount = Tasks.Count(t => t.GroupIndex == groupIndex);
                task.Name = "Task " + (taskCount + 1);
                Tasks.Add(task);
                Tasks = Tasks.OrderBy(t => t.GroupIndex).ToList();
            }

            public void DeleteTask(NewTask task)
            {
                Tasks.Remove(task);
            }

            public decimal ExpectedValue { get; set; }
            public decimal StandardDeviation { get; set; }

            public NewProject CalculateTasks()
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

                ExpectedValue = Tasks.Select(t => t.ExpectedValue).CalculateE();
                StandardDeviation = Tasks.Select(t => t.StandardDeviation).CalculateSD();
                return this;
            }

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

        public class NewTask
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

            public string ConfidenceInterval(decimal confidenceInteval)
            {
                var sdFactor = ConfidenceIntervalMaps[confidenceInteval];
                var e = ExpectedValue;
                var sd = StandardDeviation;
                var from = e - sdFactor * sd;
                var to = e + sdFactor * sd;
                return $"{from.RoundBy(0.01M)} - {to.RoundBy(0.01M)}";
            }

            public string Caption(NewProject project)
            {
                if (Type == EstimateType.Fixed)
                    return "Fixed";

                if (Type == EstimateType.ThreePoint)
                    return $"Best: {Best} | Likely: {Likely} | Worst: {Worst}";

                if (Type == EstimateType.Percentage)
                    return $"{Percentage}% of [{project.Groups[PercentageByGroupIndex]}]";

                throw new NotImplementedException();
            }
        }
    }
}
