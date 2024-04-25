using PeterDoStuff.Extensions;

namespace PeterDoStuff.Tools.Schema
{
    public abstract class Dependency
    {
        public Dependency(string leftString, string rightString)
        {
            LeftString = leftString;
            RightString = rightString;
        }

        public string LeftString
        {
            get => Left.Join(", ");
            set => Left = value.Split(",").Select(x => x.Trim()).Where(x => x.IsNullOrEmpty() == false).ToList();
        }
        public string RightString
        {
            get => Right.Join(", ");
            set => Right = value.Split(",").Select(x => x.Trim()).Where(x => x.IsNullOrEmpty() == false).ToList();
        }

        public List<string> Left { get; private set; } = new List<string>();
        public List<string> Right { get; private set; } = new List<string>();
    }
}
