using MudBlazor;
using PeterDoStuff.Extensions;
using PeterDoStuff.Graphics;
using PeterDoStuff.Graphics.Effects;
using PeterDoStuff.Graphics.Models;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.Svg
{
    public class _06_Chains : CanvasModel
    {
        public _06_Chains(int width, int height, Style? style = null) : base(width, height, style)
        {
            Name = "Chains";

            Mouse.X = 150;
            Mouse.Y = 150;

            var chain = new Chain(10, 20, ArrowJoint);
            chain.Head.Apply(new Follow(Mouse, 500));
            Add(chain);

            for (int i = 0; i < 10; i++)
            {
                var circle = CircleJoint(i);
                circle.Apply(new StickTo(chain.Joints[i]));
                Add(circle);
            }
        }

        private Arrow ArrowJoint(int i)
        {
            var arrow = new Arrow(7.5 + i*0.25, 50, 50);
            arrow.Style = Style.Clone();
            arrow.Style.StrokeWidth = 2;            
            arrow.Style.FillOpacity = 1;
            return arrow;
        }

        private CircleModel CircleJoint(int i)
        {
            var circle = new CircleModel(20 - 2*i);
            (circle.X, circle.Y) = (50, 50);
            circle.Style = Style.Clone();
            circle.Style.StrokeWidth = i > 5 ? 0 : 2;
            circle.Style.StrokeOpacity = i > 5 ? 0 : 1;
            circle.Style.FillOpacity = i > 5 ? 0 : 1;
            return circle;
        }
    }

    public class Chain : CompositeModel
    {
        public List<Model> Joints => Children;

        public Model Head => Joints.First();
        public Model Tail => Joints.Last();

        public Chain(int jointCount, double jointDistance, Func<int, Model> jointCreate, double? angleConstraint = null)
        {
            for (int i = 0 ; i < jointCount; i++)
            {
                var joint = jointCreate(i);
                Add(joint);

                if (i > 0)
                {
                    var anchor = Joints[i - 1];
                    joint.Apply(new PointTo(anchor));
                    if (angleConstraint != null)
                        joint.Apply(new AngleConstraint(anchor, angleConstraint.Value));
                    //joint.Apply(new DistanceConstraint(anchor, jointDistance));
                    joint.Apply(new ChainConstraint(anchor, jointDistance));
                }
            }
        }
    }

    public class ChainConstraint(Model anchor, double distance) : Effect
    {
        public Model Anchor = anchor;
        public double Distance = distance;

        public override void Tick()
        {
            var stickTo = new StickTo(Anchor); // Move the model next to Anchor
            stickTo.Offset.X = -Distance; // The model keep a distance behind the Anchor
            stickTo.Offset.Deg = Model.Deg - Anchor.Deg; // Rotate the offset to match with the direction Model is pointing to Anchor
            stickTo.Resolve(Model); // Update the position
        }
    }

    public class AngleConstraint(Model anchor, double angle) : Effect
    {
        public Model Anchor = anchor;
        public double Angle = angle;

        public override void Tick()
        {
            var angleDiff = (Anchor.Deg - Model.Deg).Cap(-180, 180);
            var absDiff = Math.Abs(angleDiff);
            if (absDiff > Angle)
            {
                Model.Deg = Anchor.Deg - (angleDiff / absDiff * Angle);
            }
        }
    }
}
