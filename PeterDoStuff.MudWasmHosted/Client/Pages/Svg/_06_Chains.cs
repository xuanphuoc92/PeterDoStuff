using PeterDoStuff.Extensions;
using PeterDoStuff.Graphics;
using PeterDoStuff.Graphics.Effects;
using PeterDoStuff.Graphics.Models;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.Svg
{
    public class _06_Chains : CanvasModel
    {
        public _06_Chains(Style? style = null) : base(300, 300, style)
        {
            Name = "Chains";

            Mouse.X = 150;
            Mouse.Y = 150;

            var chain1 = new Chain(10, 20, ArrowJoint);
            chain1.Head.Apply(new Follow(Mouse, 500));
            Add(chain1);

            var chain2 = new Chain(10, 20, CircleJoint);
            chain2.Head.Apply(new Follow(Mouse, 250));
            Add(chain2);
        }

        private Arrow ArrowJoint(int i)
        {
            var arrow = new Arrow(10, 50, 50);
            arrow.Style = Style.Clone();
            arrow.Style.StrokeWidth = 2;
            arrow.Style.StrokeOpacity *= 0.5 + (i * 0.05);
            arrow.Style.FillOpacity *= 0.5 + (i * 0.05);
            return arrow;
        }

        private CircleModel CircleJoint(int i)
        {
            var circle = new CircleModel(20 - 2*i);
            (circle.X, circle.Y) = (250, 250);
            circle.Style = Style.Clone();
            circle.Style.StrokeWidth = 2;
            circle.Style.StrokeOpacity *= 1 - (i * 0.05);
            circle.Style.FillOpacity *= 1 - (i * 0.05);
            return circle;
        }
    }

    public class Chain : CompositeModel
    {
        public List<Model> Joints => Children;

        public Model Head => Joints.First();

        public Chain(int jointCount, double jointDistance, Func<int, Model> jointCreate, double? angleConstraint = null)
        {
            for (int i = 0 ; i < jointCount; i++)
            {
                var joint = jointCreate(i);
                Add(joint);

                if (i > 0)
                {
                    var anchor = Joints[i - 1];
                    joint.Apply(new RotateTo(anchor));
                    if (angleConstraint != null)
                        joint.Apply(new AngleConstraint(anchor, angleConstraint.Value));
                    //joint.Apply(new DistanceConstraint(anchor, jointDistance));
                    joint.Apply(new ChainConstraint(anchor, jointDistance));
                }
            }
        }

        public class ChainConstraint(Model anchor, double distance) : Effect
        {
            public Model Anchor = anchor;
            public double Distance = distance;

            public override void Tick()
            {
                Models.ForEach(model =>
                {
                    var moveTo = new MoveTo(Anchor); // Move the model next to Anchor
                    moveTo.Offset.X = -Distance; // The model keep a distance behind the Anchor
                    moveTo.Offset.Deg = model.Deg - Anchor.Deg; // Rotate the offset to match with the direction Model is pointing to Anchor
                    moveTo.Resolve(model); // Update the position
                });
            }
        }

        public class AngleConstraint(Model anchor, double angle) : Effect 
        {
            public Model Anchor = anchor;
            public double Angle = angle;

            public override void Tick()
            {
                Models.ForEach(m =>
                {
                    var angleDiff = (Anchor.Deg - m.Deg).Cap(-180, 180);
                    var absDiff = Math.Abs(angleDiff);
                    if (absDiff > Angle)
                    {
                        m.Deg = Anchor.Deg - (angleDiff / absDiff * Angle);
                    }
                });
            }
        }
    }
}
