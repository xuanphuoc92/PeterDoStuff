using PeterDoStuff.Graphics;
using PeterDoStuff.Graphics.Effects;
using PeterDoStuff.Graphics.Models;
using System.Linq;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.Svg
{
    public class _08_Snake : CanvasModel
    {
        public _08_Snake(Style? style = null, Model? mouse = null) : base(300, 300, style, mouse)
        {
            Name = "Snake";

            var spine = new Chain(30, 15, i => CreateSpine(i), 45);
            spine.Head.Apply(new Follow(Mouse, 250));
            Add(spine);

            var curve = new CurveModel();

            // Draw left side
            for(int i = 0; i < spine.Joints.Count; i++)
            {
                var leftPoint = new Model();
                var joint = spine.Joints[i];
                var moveTo = new MoveTo(joint);
                moveTo.Offset.Y = -GetBodySize(i);
                leftPoint.Apply(moveTo);
                curve.CurveTo(leftPoint);
            }

            // Draw tail
            var tailPoint = new Model();
            var tailJoint = spine.Joints.Last();
            var stickToTail = new MoveTo(tailJoint);
            stickToTail.Offset.X = -4;
            tailPoint.Apply(stickToTail);
            curve.CurveTo(tailPoint);

            // Draw right side
            for (int  i = spine.Joints.Count - 1; i >= 0; i--)
            {
                var rightPoint = new Model();
                var joint = spine.Joints[i];
                var moveTo = new MoveTo(joint);
                moveTo.Offset.Y = GetBodySize(i);
                rightPoint.Apply(moveTo);
                curve.CurveTo(rightPoint);
            }

            AddAndStyle(curve);
            curve.Style.StrokeWidth = 1;
        }

        private static double GetBodySize(int i)
        {
            if (i == 0)
                return 10;
            return 16 - i * 0.5;
        }

        private Model CreateSpine(int i)
        {
            var joint = new CircleModel(GetBodySize(i));
            joint.Style = Style.Clone();
            joint.Style.StrokeWidth = 2;
            joint.Style.StrokeOpacity = 0.2;
            joint.Style.FillOpacity = 0;
            return joint;
        }
    }

    public class CurveModel : CompositeModel
    {
        public List<Model> Points => Children;

        public CurveModel CurveTo(Model point)
        {
            Add(point);
            return this;
        }
    }
}
