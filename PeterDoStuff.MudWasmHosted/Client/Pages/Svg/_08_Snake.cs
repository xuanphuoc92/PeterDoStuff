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

            var spine = new Chain(30, 15, _ => new Model(), 45);
            spine.Head.Apply(new Follow(Mouse, 250));
            Add(spine);

            var curve = new CurveModel();

            var headPoint = new Model();            
            var stickToHead = new MoveTo(spine.Head);
            stickToHead.Offset.X = GetBodySize(0);
            headPoint.Apply(stickToHead);
            curve.CurveTo(headPoint);

            // Draw left side
            for (int i = 0; i < spine.Joints.Count; i++)
            {
                var leftPoint = new Model();
                var joint = spine.Joints[i];
                var moveTo = new MoveTo(joint);
                moveTo.Offset.X = GetBodySize(i);
                moveTo.Offset.Deg = -GetBodyAngle(i);
                leftPoint.Apply(moveTo);
                curve.CurveTo(leftPoint);
            }

            // Connect tail
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
                moveTo.Offset.X = GetBodySize(i);
                moveTo.Offset.Deg = GetBodyAngle(i);
                rightPoint.Apply(moveTo);
                curve.CurveTo(rightPoint);
            }

            // Connect back to head
            curve.CurveTo(headPoint);

            AddAndStyle(curve);
            curve.Style.StrokeWidth = 1;
            curve.Style.FillOpacity = 0.2;

            // Draw eyes
            CreateEye(spine.Joints[1], -45);            
            CreateEye(spine.Joints[1], 45);
        }

        private CircleModel CreateEye(Model joint, double angle)
        {
            var eye = new CircleModel(4);
            AddAndStyle(eye);
            eye.Style.StrokeWidth = 0;
            eye.Style.FillOpacity = 1;
            var stickTo = new MoveTo(joint);
            var i = 1;
            stickTo.Offset.X = (16 - i * 0.5) - 2.5;
            stickTo.Offset.Deg = angle;
            eye.Apply(stickTo);
            return eye;
        }

        private static double GetBodySize(int i)
        {
            if (i == 0)
                return 10;
            return 16 - i * 0.5;
        }

        private static double GetBodyAngle(int i)
        {
            if (i == 0)
                return 60;
            return 90;
        }

        //private Model CreateSpine(int i)
        //{
        //    var joint = new CircleModel(GetBodySize(i));
        //    joint.Style = Style.Clone();
        //    joint.Style.StrokeWidth = 2;
        //    joint.Style.StrokeOpacity = 0.2;
        //    joint.Style.FillOpacity = 0;
        //    return joint;
        //}
    }

    public class CurveModel : CompositeModel
    {
        public List<Model> Points => Children;

        public CurveModel CurveTo(Model point)
        {
            Add(point);
            return this;
        }

        public bool IsClosed = false;

        public CurveModel Close()
        {
            IsClosed = true;
            return this;
        }
    }
}
