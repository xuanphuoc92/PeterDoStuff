using PeterDoStuff.Extensions;
using PeterDoStuff.Graphics;
using PeterDoStuff.Graphics.Effects;
using PeterDoStuff.Graphics.Models;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.Svg
{
    public class _09_Fish : CanvasModel
    {
        public _09_Fish(int width, int height, Style? style = null) : base(width, height, style, null)
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

            // Pectorial Fins
            CreateSideFin(spine, 3, pectoralFinX, pectoralFinY, -60, 45);
            CreateSideFin(spine, 3, pectoralFinX, pectoralFinY, 60, -45);

            // Ventral Fins
            CreateSideFin(spine, 7, ventralFinX, ventralFinY, -90, 45);
            CreateSideFin(spine, 7, ventralFinX, ventralFinY, 90, -45);

            var caudalFin = new CurveModel();
            var startCaudalIndex = bodyWidth.Length - 2;
            var endCaudalIndex = spine.Joints.Count - 1;
            for (int i = startCaudalIndex; i <= endCaudalIndex; i++)
            {
                var caudalFinPoint = new Model();
                caudalFinPoint.Apply(new CaudalFinEffectBottom(spine, i, startCaudalIndex, scale * caudalFinSize));
                caudalFin.CurveTo(caudalFinPoint);
            }
            for (int i = endCaudalIndex; i >= startCaudalIndex; i--)
            {
                var caudalFinPoint = new Model();
                caudalFinPoint.Apply(new CaudalFinEffectBottom(spine, i, startCaudalIndex, -scale * caudalFinSize));
                caudalFin.CurveTo(caudalFinPoint);
            }
            AddAndStyle(caudalFin);
            caudalFin.Style.StrokeWidth = 1;
            caudalFin.Style.FillColor = caudalFin.Style.StrokeColor;

            var body = new CurveModel();

            // Head:
            body.CurveTo(
                anchor: spine.Head,
                offset: new Model() { X = GetBodyWidth(0), Deg = 0 });
            body.CurveTo(
                anchor: spine.Head,
                offset: new Model() { X = GetBodyWidth(0), Deg = -30 });

            // Left side:
            for (int i = 0; i <= bodyWidth.Length - 1; i++)
            {
                body.CurveTo(
                    anchor: spine.Joints[i],
                    offset: new Model() { X = GetBodyWidth(i), Deg = -90 });
            }

            // Tail:
            body.CurveTo(spine.Joints[bodyWidth.Length - 1]);

            // Right side:
            for (int i = bodyWidth.Length - 1; i >= 0; i--)
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

            // Dorsal fin:
            var dorsalFin = new PathModel();
            dorsalFin.MoveTo(spine.Joints[4]);
            dorsalFin.CurveTo(spine.Joints[5], spine.Joints[6], spine.Joints[7]);

            var dorsalFinPoint1 = new Model();
            dorsalFinPoint1.Apply(new DorsalFinEffect(spine, 6, -scale * dorsalFinSize));
            var dorsalFinPoint2 = new Model();
            dorsalFinPoint2.Apply(new DorsalFinEffect(spine, 5, -scale * dorsalFinSize));
            Add(dorsalFinPoint1);
            Add(dorsalFinPoint2);

            dorsalFin.CurveTo(dorsalFinPoint1, dorsalFinPoint2, spine.Joints[4]);

            AddAndStyle(dorsalFin);
            dorsalFin.Style.StrokeWidth = 1;
            dorsalFin.Style.FillColor = dorsalFin.Style.StrokeColor;
            dorsalFin.Style.FillOpacity = 0.2;
        }

        private void CreateSideFin(Chain spine, int jointIndex, double finX, double finY, double stickAngle, double pointAngle)
        {
            var fin = new EllipseModel(
                rx: finX * scale / 2,
                ry: finY * scale / 2);

            var stickTo = new StickTo(spine.Joints[jointIndex]);
            stickTo.Offset.X = GetBodyWidth(jointIndex);
            stickTo.Offset.Deg = stickAngle;
            fin.Apply(stickTo);

            var pointTo = new PointTo(spine.Joints[jointIndex - 1], PointMode.Copy);
            pointTo.Offset.Deg = pointAngle;
            fin.Apply(pointTo);

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
        private double ventralFinX = 96;
        private double ventralFinY = 32;
        private double eyeSize = 20;
        private double caudalFinSize = 2.5;
        private double dorsalFinSize = 24;

        private double GetJointDistance() => jointDistance * scale;
        private double GetBodyWidth(int i) => i < bodyWidth.Length ? bodyWidth[i] * scale : 0;
        private double GetEyeSize() => eyeSize * scale;

        private class CaudalFinEffectBottom(Chain spine, int jointIndex, int startCaudalIndex, double finSize) : Effect
        {
            public Chain Spine = spine;
            public int JointIndex = jointIndex;
            public int StartIndex = startCaudalIndex;
            public double Size = finSize;

            public override void Tick()
            {
                var baseSize = Size * (JointIndex - StartIndex) * (JointIndex - StartIndex);

                var midIndex = Spine.Joints.Count / 2;
                var headToMid = (Spine.Head.Deg - Spine.Joints[midIndex].Deg).Cap(-180, 180);
                var midToTail = (Spine.Joints[midIndex].Deg - Spine.Tail.Deg).Cap(-180, 180);
                var turnSize = (headToMid + midToTail) / 180 * baseSize;

                var joint = Spine.Joints[JointIndex];
                var stickTo = new StickTo(joint);
                stickTo.Offset.Y = turnSize;
                stickTo.Resolve(Model);
            }
        }

        private class CaudalFinEffectTop(Chain spine, int jointIndex, int startCaudalIndex, double finSize) : Effect
        {
            public Chain Spine = spine;
            public int JointIndex = jointIndex;
            public int StartIndex = startCaudalIndex;
            public double Size = finSize;

            public override void Tick()
            {
                var baseSize = Size * (JointIndex - StartIndex) * (JointIndex - StartIndex);

                //var midIndex = Spine.Joints.Count / 2;
                //var headToMid = (Spine.Head.Deg - Spine.Joints[midIndex].Deg).Cap(-180, 180);
                //var midToTail = (Spine.Joints[midIndex].Deg - Spine.Tail.Deg).Cap(-180, 180);
                //var turnSize = (headToMid + midToTail) / 180 * 1.5 * baseSize;

                var joint = Spine.Joints[JointIndex];
                var stickTo = new StickTo(joint);
                stickTo.Offset.Y = baseSize;
                stickTo.Resolve(Model);
            }
        }

        private class DorsalFinEffect(Chain spine, int jointIndex, double finSize) : Effect
        {
            public Chain Spine = spine;
            public int JointIndex = jointIndex;
            public double Size = finSize;

            public override void Tick()
            {
                var joint = Spine.Joints[JointIndex];
                var angleControlJoint = Spine.Joints[JointIndex + 1];
                var headToJoint = (Spine.Head.Deg - angleControlJoint.Deg).Cap(-180, 180);

                var stickTo = new StickTo(joint);
                stickTo.Offset.Y = Size * headToJoint / 180 * 1.5;
                stickTo.Resolve(Model);
            }
        }

    }
}
