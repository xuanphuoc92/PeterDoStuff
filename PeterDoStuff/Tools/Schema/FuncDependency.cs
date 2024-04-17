namespace PeterDoStuff.Tools.Schema
{
    public class FuncDependency : Dependency
    {
        public FuncDependency(string leftString, string rightString) : base(leftString, rightString)
        {
        }

        public bool IsTrivial()
        {
            foreach (var right in Right)
            {
                if (!Left.Contains(right))
                    return false;
            }
            return true;
        }

        public bool IsNonTrivial()
            => IsTrivial() == false;

        public bool IsCompletelyNonTrivial()
        {
            if (IsTrivial() == true) return false;

            foreach (var right in Right)
                if (Left.Contains(right)) return false;

            return true;
        }
    }
}
