using PeterDoStuff.Graphics;
using PeterDoStuff.Graphics.Effects;
using PeterDoStuff.Graphics.Models;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.Svg
{
    public class _09_Fish : CanvasModel
    {
        public _09_Fish(Style? style = null, Model? mouse = null) : base(300, 300, style, mouse)
        {
            Name = "Fish";

            var spine = new Chain(
                jointCount: 12,
                jointDistance: GetJointDistance(),
                jointCreate: _ => new Model(),
                //jointCreate: JointOutline,
                angleConstraint: 22.5);

            spine.Head.Apply(new Follow(Mouse, 250));
            Add(spine);

            // Pectorial Fins (side fins)
            var pectorialFinJoint = spine.Joints[3];
            var pectorialFinAnchor = spine.Joints[2];
            // Left Fin
            CreatePectoralFin(pectorialFinJoint, -60, pectorialFinAnchor, 45);
            // Right Fin
            CreatePectoralFin(pectorialFinJoint, 60, pectorialFinAnchor, -45);

            var body = new CurveModel();

            // Head:
            body.CurveTo(
                anchor: spine.Head,
                offset: new Model() { X = GetBodyWidth(0), Deg = 0 });
            body.CurveTo(
                anchor: spine.Head,
                offset: new Model() { X = GetBodyWidth(0), Deg = -30 });

            // Left side:
            for (int i = 0; i < bodyWidth.Length - 2; i++)
            {
                body.CurveTo(
                    anchor: spine.Joints[i],
                    offset: new Model() { X = GetBodyWidth(i), Deg = -90 });
            }

            // Tail:
            body.CurveTo(spine.Joints[bodyWidth.Length - 1]);

            // Right side:
            for (int i = bodyWidth.Length - 3; i >= 0; i--)
            {
                body.CurveTo(
                    anchor: spine.Joints[i],
                    offset: new Model() { X = GetBodyWidth(i), Deg = 90 });
            }

            // Back to Head:
            body.CurveTo(
                anchor: spine.Head,
                offset: new Model() { X = GetBodyWidth(0), Deg = 30 });
            body.CurveTo(
                anchor: spine.Head,
                offset: new Model() { X = GetBodyWidth(0), Deg = 0 });
            body.Close();

            AddAndStyle(body);
            body.Style.StrokeWidth = 1;
            body.Style.FillOpacity = 1;

            // Draw eyes
            CreateEye(spine.Joints[0], -90);
            CreateEye(spine.Joints[0], 90);
        }

        private void CreatePectoralFin(Model finJoint, double stickAngle, Model finAnchor, double pointAngle)
        {
            var fin = new EllipseModel(
                rx: GetPectorialFinX(),
                ry: GetPectorialFinY());

            var stickLeftFin = new StickTo(finJoint);
            stickLeftFin.Offset.X = GetBodyWidth(3);
            stickLeftFin.Offset.Deg = stickAngle;
            fin.Apply(stickLeftFin);

            var pointLeftFin = new PointTo(finAnchor, PointMode.Mirrow);
            pointLeftFin.Offset.Deg = pointAngle;
            fin.Apply(pointLeftFin);

            AddAndStyle(fin);
            fin.Style.StrokeWidth = 1;
            fin.Style.FillColor = fin.Style.StrokeColor;
            fin.Style.FillOpacity = 0.2;
        }

        private CircleModel CreateEye(Model joint, double angle)
        {
            var eye = new CircleModel(GetEyeSize());
            AddAndStyle(eye);
            eye.Style.StrokeWidth = 0;
            eye.Style.FillColor = eye.Style.StrokeColor;
            eye.Style.FillOpacity = 1;

            var stickTo = new StickTo(joint);
            var i = 0;
            stickTo.Offset.X = (bodyWidth[i] - 18) * scale;
            stickTo.Offset.Deg = angle;
            eye.Apply(stickTo);

            return eye;
        }

        private CircleModel JointOutline(int i)
        {           
            var joint = new CircleModel(GetBodyWidth(i));
            joint.Style = Style.Clone();
            joint.Style.StrokeOpacity = 1;
            joint.Style.FillOpacity = 0.2;
            return joint;
        }

        private double[] bodyWidth = [68, 81, 84, 83, 77, 64, 51, 38, 32, 19];
        private double jointDistance = 64;
        private double scale = 0.3;
        private double pectoralFinX = 160;
        private double pectoralFinY = 64;
        private double eyeSize = 20;

        private double GetJointDistance() => jointDistance * scale;
        private double GetBodyWidth(int i) => i < bodyWidth.Length ? bodyWidth[i] * scale : 0;
        private double GetPectorialFinX() => pectoralFinX * scale / 2;
        private double GetPectorialFinY() => pectoralFinY * scale / 2;
        private double GetEyeSize() => eyeSize * scale;
    }

    public class EllipseModel(double rx, double ry) : Model
    {
        public double Rx = rx, Ry = ry;
    }
}
