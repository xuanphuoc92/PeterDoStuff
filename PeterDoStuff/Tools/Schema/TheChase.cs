

using PeterDoStuff.Extensions;
using System.Data;

namespace PeterDoStuff.Tools.Schema
{
    public class TheChase
    {
        public List<string> Schema { get; private set; } = new();
        public List<Dependency> Dependencies { get; private set; } = new();
        public List<string> Decomposition1 { get; private set; } = new();
        public List<string> Decomposition2 { get; private set; } = new();

        public bool Lossless1 { get; private set; }
        public bool Lossless2 { get; private set; }
        public string Logs { get; private set; }
        public void Chase()
        {
            Logs = "";

            Log($"Schema: {Schema.Join(", ")}");
            Log("Dependencies:");
            Dependencies.ForEach(d => Log(d.ToString()));
            Log($"Decomposition 1: {Decomposition1.Join(", ")}");
            Log($"Decomposition 2: {Decomposition2.Join(", ")}");

            LogSeparator('=');

            var intersect = Decomposition1.Intersect(Decomposition2).ToList();
            string intersectString = intersect.Join(", ");
            Log($"Intersect of Decompositions: {intersectString}");
            var chaseDependency1 = new MultiValDependency(intersectString, Decomposition1.Except(intersect).Join(","));
            var chaseDependency2 = new MultiValDependency(intersectString, Decomposition2.Except(intersect).Join(","));
            Log("Chase Dependencies (Intesect ->> Decomposition except Intersect):");
            Log(chaseDependency1.ToString());
            Log(chaseDependency2.ToString());

            LogSeparator('=');
            Log($"Chase Depedency 1: {chaseDependency1}");
            Lossless1 = Chase(chaseDependency1);

            LogSeparator('=');
            Log($"Chase Depedency 2: {chaseDependency2}");
            Lossless2 = Chase(chaseDependency2);
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
                    var satisfy = TableSatisfiesDependency(table, dependency);
                    allSatisfy &= satisfy;
                    Log($"> Check dependency {dependency}: {satisfy}");
                }

                if (allSatisfy)
                {
                    Log("Table satisifes all dependencies. End step 3.");
                    break;
                }
            } while (false);

            return false;
        }

        private bool TableSatisfiesDependency(List<Dictionary<string, string>> table, Dependency dependency)
        {
            foreach (var row in table)
            {
                var queriedRows = table
                    .Where(qrow => dependency.Left
                        .All(left => qrow[left] == row[left]))
                    .Where(qrow => dependency.Right.Except(dependency.Left)
                        .Any(right => qrow[right] != row[right]))
                    .ToList();

                if (queriedRows.Any() == false)
                    continue;

                if (dependency is FuncDependency)
                    return false;

                if (dependency is MultiValDependency)
                {
                    var zAttributes = Schema.Except(dependency.Left.Union(dependency.Right));
                    if (zAttributes.Any() == false)
                        continue;

                    var zRows = queriedRows
                        .Where(qrow => zAttributes.All(z => qrow[z] == row[z]));

                    if (zRows.Any() == false)
                        return false;
                }
            }

            return true;
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
    }
}
