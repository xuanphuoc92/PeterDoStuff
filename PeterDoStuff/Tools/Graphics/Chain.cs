namespace PeterDoStuff.Tools.Graphics
{
    public class Chain : CompositeModel
    {
        public List<Model> Joints = [];

        public Chain(int jointCount, double jointDistance, Func<Model> jointModel)
            : this(jointCount, jointDistance, _ => jointModel())
        {
        }

        public Chain(int jointCount, double jointDistance, Func<int, Model> jointModel)
        {
            for (int i = 0; i < jointCount; i++)
            {
                var joint = jointModel(i);
                Joints.Add(joint);
                Models.Add(joint);

                if (i > 0)
                {
                    joint.AddEffect(new DistanceConstraint(Joints[i - 1], jointDistance, jointDistance));
                }
            }
        }
    }
}
