namespace PeterDoStuff.Tools.Schema
{
    public class FuncDependency : Dependency
    {
        public FuncDependency(string leftString, string rightString) : base(leftString, rightString)
        {
        }

        public override string ToString() => $"{LeftString} -> {RightString}";

        /// <summary>
        /// A functional dependency X (left) --> Y (right) is trivial if and only if Y is a subset of X.
        /// </summary>
        /// <returns></returns>
        public bool IsTrivial()
        {
            return Right.Any(right => Left.Contains(right) == false) == false;
        }

        /// <summary>
        /// A functional dependency X (left) --> Y (right) is trivial if and only if Y is a subset of X.
        /// </summary>
        /// <returns></returns>
        public bool IsNonTrivial()
            => IsTrivial() == false;

        /// <summary>
        /// A functional dependency X (left) --> Y (right) is completely non-trivial if and only if:
        /// 1. Y is not empty and intersection of X and Y is empty
        /// or
        /// 2. X --> Y is non-trivial and intersection of X and Y is empty
        /// </summary>
        /// <returns></returns>
        public bool IsCompletelyNonTrivial()
        {
            if (IsTrivial() == true) return false;

            return Left.Any(left => Right.Contains(left)) == false;
        }
    }
}
