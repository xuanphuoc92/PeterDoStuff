

using PeterDoStuff.Extensions;

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
            return false;
        }

        private void LogSeparator(char separatorChar) => Log(new string(separatorChar, 70));

        private void Log(string line) => Logs += line + Environment.NewLine;
    }
}
