

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
            Log($"Decomposition 2: {Decomposition1.Join(", ")}");
        }

        private void Log(string line)
        {
            Logs += line + Environment.NewLine;
        }
    }
}
