namespace PeterDoStuff.Tools.Graphics
{
    public class Chain : CompositeModel
    {
        public List<Model> Joints => Models;

        public Chain(int jointCount, double jointDistance, Func<Model> jointModel)
            : this(jointCount, jointDistance, _ => jointModel())
        {
        }

        public Chain(int jointCount, double jointDistance, Func<int, Model> jointModel)
        {
            for (int i = 0; i < jointCount; i++)
            {
                var joint = jointModel(i);
                Add(joint);

                if (i > 0)
                {
                    Model anchor = Joints[i - 1];
                    joint.AddEffect(new PointTo(anchor));
                    joint.AddEffect(new DistanceConstraint(anchor, jointDistance, jointDistance));
                }
            }
        }
    }
}
