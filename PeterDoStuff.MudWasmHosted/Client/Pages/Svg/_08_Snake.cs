using PeterDoStuff.Graphics;
using PeterDoStuff.Graphics.Effects;
using PeterDoStuff.Graphics.Models;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.Svg
{
    public class _08_Snake : CanvasModel
    {
        public _08_Snake(int width, int height, Style? style = null, Model? mouse = null) : base(width, height, style, mouse)
        {
            Name = "Snake";

            var spine = new Chain(60, 15, _ => new Model(), 45);
            spine.Head.Apply(new Follow(Mouse, 250));
            Add(spine);

            var curve = new CurveModel();
            
            // Start and record the Head Point to later connect back
            var headPoint = curve.CurveTo(
                anchor: spine.Head, 
                offset: new Model() { X = GetBodySize(0) });

            // Draw left side
            for (int i = 0; i < spine.Joints.Count; i++)
            {
                curve.CurveTo(
                    anchor: spine.Joints[i], 
                    offset: new Model() { X = GetBodySize(i), Deg = -GetBodyAngle(i) });
            }

            // Connect tail
            curve.CurveTo(
                anchor: spine.Tail,
                offset: new Model() { X = -4 });

            // Draw right side
            for (int  i = spine.Joints.Count - 1; i >= 0; i--)
            {
                curve.CurveTo(
                    anchor: spine.Joints[i],
                    offset: new Model() { X = GetBodySize(i), Deg = GetBodyAngle(i) });
            }

            // Connect back to head
            curve.CurveTo(headPoint);
            curve.Close();

            AddAndStyle(curve);
            curve.Style.StrokeWidth = 1;
            curve.Style.FillOpacity = 1;

            // Draw eyes
            CreateEye(spine.Joints[1], -45);            
            CreateEye(spine.Joints[1], 45);
        }

        private CircleModel CreateEye(Model joint, double angle)
        {
            var eye = new CircleModel(4);
            AddAndStyle(eye);
            eye.Style.StrokeWidth = 0;
            eye.Style.FillColor = eye.Style.StrokeColor;
            eye.Style.FillOpacity = 1;

            var stickTo = new StickTo(joint);
            var i = 1;
            stickTo.Offset.X = GetBodySize(i) - 2.5;
            stickTo.Offset.Deg = angle;
            eye.Apply(stickTo);

            return eye;
        }

        private static double GetBodySize(int i)
        {
            if (i == 0)
                return 10;
            return 16 - i * 0.25;
        }

        private static double GetBodyAngle(int i)
        {
            if (i == 0)
                return 60;
            return 90;
        }
    }
}
