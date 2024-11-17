using PeterDoStuff.Tools.Graphics;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.SVG
{
    public class DemoSnake : Canvas
    {
        public DemoSnake(Style? style = null) : base(300, 300, style)
        {
            Name = "Snake";

            var chain = new Chain(24, 15, CreateJoint);
            Add(chain);

            var spineLine = CreateLine(chain);
            Add(spineLine);

            var head = chain.Joints[0];
            head.AddEffect(new Follow(Mouse) { Velocity = 1000, SlowRange = 100 });
        }

        private PathModel CreateLine(Chain chain)
        {
            var pathModel = new PathModel(0, 0);
            pathModel.Style = Style.Clone();
            pathModel.Style.StrokeWidth = 2;
            pathModel.Style.FillOpacity = 0;
            pathModel.Models[0].References.Add(chain.Joints[0]);

            for (int i = 1; i < chain.Joints.Count; i++)
            {
                pathModel.LineTo(0, 0);
                pathModel.Models.Last().References.Add(chain.Joints[i]);
            }
            return pathModel;
        }

        private CompositeModel CreateJoint(int index)
        {
            var joint = new CompositeModel();

            var jointShape = new Circle(64 / 4);
            jointShape.Style = Style.Clone();
            jointShape.Style.StrokeWidth = 2;
            jointShape.Style.StrokeOpacity = 0.2;
            jointShape.Style.FillOpacity = 0;

            joint.Add(jointShape);
            
            return joint;
        }
    }
}
