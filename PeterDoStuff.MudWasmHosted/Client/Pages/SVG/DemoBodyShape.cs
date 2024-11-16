using PeterDoStuff.Tools.Graphics;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.SVG
{
    public class DemoBodyShape : Canvas
    {
        public DemoBodyShape(Style? style = null) : base(300, 300, style)
        {
            Name = "Body Shape";

            var chain = new Chain(shapes.Length, 15, CreateJoint);
            Add(chain);

            var head = chain.Joints[0];
            head.AddEffect(new Follow(Mouse) { Velocity = 1000, SlowRange = 100 });
        }

        double[] shapes = [68, 84, 87, 85, 83, 77, 64, 60, 51, 38, 34, 32, 19, 15];
        private CompositeModel CreateJoint(int index)
        {
            var joint = new CompositeModel();
            
            var jointShape = new Circle(shapes[index] / 4);
            jointShape.Style = Style.Clone();
            jointShape.Style.StrokeWidth = 2;
            jointShape.Style.FillOpacity = 0;
            
            joint.Models.Add(jointShape);

            joint.AddEffect(j => jointShape.MoveTo(j));
            return joint;
        }
    }
}
