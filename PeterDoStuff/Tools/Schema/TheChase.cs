

using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PeterDoStuff.Extensions;
using System.Data;

namespace PeterDoStuff.Tools.Schema
{
    public class TheChase
    {
        public List<string> Schema { get; private set; } = new();
        public List<Dependency> Dependencies { get; private set; } = new();
        public string Logs { get; private set; }

        private void InitLogs()
        {
            Logs = "";
            Log($"Schema: {Schema.Join(", ")}");
            Log("Dependencies:");
            Dependencies.ForEach(d => Log(d.ToString()));
        }

        public bool ChaseDependency(Dependency chaseDependency)
        {
            InitLogs();
            Log($"Chase Dependency: {chaseDependency}");
            LogSeparator('=');
            return Chase(chaseDependency);
        }

        public (bool Lossless1, bool Lossless2) ChaseDecompositions(IEnumerable<string> decomposition1, IEnumerable<string> decomposition2)
        {
            InitLogs();
            Log($"Decomposition 1: {decomposition1.Join(", ")}");
            Log($"Decomposition 2: {decomposition2.Join(", ")}");

            LogSeparator('=');

            var intersect = decomposition1.Intersect(decomposition2).ToList();
            string intersectString = intersect.Join(", ");
            Log($"Intersect of Decompositions: {intersectString}");
            var chaseDependency1 = new MultiValDependency(intersectString, decomposition1.Except(intersect).Join(","));
            var chaseDependency2 = new MultiValDependency(intersectString, decomposition2.Except(intersect).Join(","));
            Log("Chase Dependencies (Intesect ->> Decomposition except Intersect):");
            Log(chaseDependency1.ToString());
            Log(chaseDependency2.ToString());

            LogSeparator('=');
            Log($"Chase Depedency 1: {chaseDependency1}");
            var lossless1 = Chase(chaseDependency1);
            Log($"The decomposition is {(lossless1 ? "Lossless" : "Lossy")}");

            LogSeparator('=');
            Log($"Chase Depedency 2: {chaseDependency2}");
            var lossless2 = Chase(chaseDependency2);
            Log($"The decomposition is {(lossless2 ? "Lossless" : "Lossy")}");
            return (lossless1, lossless2);
        }

        private bool Chase(Dependency chaseDependency)
        {
            Log("Step 1: Initialize table with 2 rows of distinct values");
            List<Dictionary<string, string>> table = new();
            AddRow(table, 1);
            AddRow(table, 2);
            LogTable(table);
            Log();

            Log($"Step 2: For each attribute in Left ({chaseDependency.LeftString}), make their values the same");
            foreach (var left in chaseDependency.Left)
            {
                var row1 = table[0];
                var row2 = table[1];
                row2[left] = row1[left];
            }
            LogTable(table);
            Log();

            Log("Step 3: Check each dependency and update table until table satisifies all dependencies");
            do
            {
                bool allSatisfy = true;
                foreach (var dependency in Dependencies)
                {
                    Log($"> Check dependency {dependency}:");
                    var satisfy = TableSatisfiesDependency(table, dependency);
                    allSatisfy &= satisfy;
                    if (satisfy == false)
                    {
                        Log("Update table to satisfy dependency");
                        UpdateTableToSatisfyDependency(table, dependency);
                    }
                }

                if (allSatisfy)
                {
                    Log("Table satisifes all dependencies. End step 3.");
                    break;
                }
            } while (true);
            Log();

            Log($"Step 4: Check if table satisfies the chase depedency {chaseDependency}");
            return TableSatisfiesDependency(table, chaseDependency);
        }

        private void UpdateTableToSatisfyDependency(List<Dictionary<string, string>> table, Dependency dependency)
        {
            var count = table.Count;
            for (int i = 0; i < count; i++)
            {
                var row = table[i];
                var queriedRows = QuerySameInLeftAndDifferentInRight(table, dependency, row);

                if (queriedRows.Any() == false)
                    continue;

                if (dependency is FuncDependency)
                {
                    foreach (var qrow in queriedRows)
                    {
                        Log($"Update row {table.IndexOf(qrow) + 1}:");
                        foreach (var right in dependency.Right.Except(dependency.Left))
                        {
                            //if (qrow[right] == row[right])
                            //    continue;
                            Log($"[{right}]: {qrow[right]} -> {row[right]}");
                            qrow[right] = row[right];
                        }
                    }
                }
                else // if (dependency is MultiValDependency)
                {
                    var zAttributes = Schema.Except(dependency.Left.Union(dependency.Right));
                    
                    //if (zAttributes.Any() == false)
                    //    continue;

                    //var zRows = queriedRows
                    //    .Where(qrow => zAttributes.All(z => qrow[z] == row[z]));

                    //if (zRows.Any())
                    //    continue;

                    foreach (var qrow in queriedRows)
                    {
                        var newRow = new Dictionary<string, string>();
                        dependency.Left
                            .ForEach(left => newRow[left] = row[left]);
                        dependency.Right.Except(dependency.Left).ToList()
                            .ForEach(right => newRow[right] = qrow[right]);
                        zAttributes.ToList()
                            .ForEach(z => newRow[z] = row[z]);

                        Log($"New Row Add:");
                        LogRow(newRow);
                        table.Add(newRow);
                    }
                }
            }

            Log("Updated table:");
            LogTable(table);
        }

        private bool TableSatisfiesDependency(List<Dictionary<string, string>> table, Dependency dependency)
        {
            foreach (var row in table)
            {
                var queriedRows = QuerySameInLeftAndDifferentInRight(table, dependency, row);

                if (queriedRows.Any() == false)
                    continue;

                if (dependency is FuncDependency)
                {
                    Log("Not Satisfied");
                    return false;
                }
                else // if (dependency is MultiValDependency)
                {
                    var zAttributes = Schema.Except(dependency.Left.Union(dependency.Right));
                    if (zAttributes.Any() == false)
                        continue;

                    var zRows = queriedRows
                        .Where(qrow => zAttributes.All(z => qrow[z] == row[z]));

                    if (zRows.Any() == false)
                    {
                        Log("Not Satisfied");
                        return false;
                    }
                }
            }

            Log("Satisfied");
            return true;
        }

        private static List<Dictionary<string, string>> QuerySameInLeftAndDifferentInRight(List<Dictionary<string, string>> table, Dependency dependency, Dictionary<string, string> row)
        {
            return table
                .Where(qrow => dependency.Left
                    .All(left => qrow[left] == row[left]))
                .Where(qrow => dependency.Right.Except(dependency.Left)
                    .Any(right => qrow[right] != row[right]))
                .ToList();
        }

        private void AddRow(List<Dictionary<string, string>> table, int number)
        {
            var row = new Dictionary<string, string>();
            for (int i = 0; i < Schema.Count; i++)
            {
                var col = Schema[i];
                row[col] = ColumnText(i) + number;
            }
            table.Add(row);
        }

        private string ColumnText(int i)
        {
            string result = "";
            while (i >= 0)
            {
                result = (char)('a' + i % 26) + result;
                i = i / 26 - 1;
            }
            return result;
        }


        private void LogSeparator(char separatorChar) => Log(new string(separatorChar, 70));

        private void Log() => Log("");

        private void Log(string line) => Logs += line + Environment.NewLine;

        private void LogTable(List<Dictionary<string, string>> table)
        {
            // Calculate column sizes
            Dictionary<string, int> colSizes = new();
            foreach (var col in Schema)
            {
                var rowSizes = table.Select(r => r[col].Length).ToList();
                rowSizes.Add(col.Length);
                colSizes[col] = rowSizes.Max();
            }

            // Log the Header
            string header = Schema.Select(c => c.PadRight(colSizes[c])).Join(" | ");
            Log(header);
            Log(new string('-', header.Length));

            // Log the rows
            foreach (var row in table)
                Log(Schema.Select(col => row[col].PadRight(colSizes[col])).Join(" | "));
        }

        private void LogRow(Dictionary<string, string> row)
        {
            // Calculate column sizes
            Dictionary<string, int> colSizes = new();
            foreach (var col in Schema)
            {
                var rowSizes = new List<int>
                {
                    row[col].Length,
                    col.Length
                };
                colSizes[col] = rowSizes.Max();
            }

            // Log the Header
            string header = Schema.Select(c => c.PadRight(colSizes[c])).Join(" | ");
            Log(header);

            // Log the rows
            Log(Schema.Select(col => row[col].PadRight(colSizes[col])).Join(" | "));
        }
    }
}
