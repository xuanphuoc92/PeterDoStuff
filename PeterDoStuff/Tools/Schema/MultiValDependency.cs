namespace PeterDoStuff.Tools.Schema
{
    public class MultiValDependency : Dependency
    {
        public MultiValDependency(string leftString, string rightString) : base(leftString, rightString)
        {
        }

        public override string ToString() => $"{LeftString} ->> {RightString}";

        /// <summary>
        /// A multi-valued dependency X (left) ->> Y (right) if and only if:
        /// 1. Y = R - X
        /// or
        /// 2. Y is subset of X
        /// </summary>
        /// <returns></returns>
        public bool IsTrivial(params string[] otherAttributes)
        {
            // 2. Y is subset of X
            bool isSubset = Right.All(r => Left.Contains(r));
            if (isSubset) return true;

            // 1. Y = R - X
            var relation = Left.Union(Right).Union(otherAttributes).Distinct();
            var Y = relation.Except(Left);
            bool isYandRightSame = Y.OrderBy(y => y).SequenceEqual(Right.OrderBy(r => r));
            return isYandRightSame;
        }

        /// <summary>
        /// A multi-valued dependency X (left) ->> Y (right) if and only if:
        /// 1. Y = R - X
        /// or
        /// 2. Y is subset of X
        /// </summary>
        /// <returns></returns>
        public bool IsNonTrivial(params string[] otherAttributes) 
            => IsTrivial(otherAttributes) == false;
    }
}
