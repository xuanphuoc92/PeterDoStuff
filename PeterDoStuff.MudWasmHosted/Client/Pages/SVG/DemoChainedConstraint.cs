using PeterDoStuff.Tools.Graphics;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.SVG
{
    public class DemoChainedConstraint : Canvas
    {
        public DemoChainedConstraint(Style? style = null) : base(300, 300, style)
        {
            Name = "Chained Constraint";

            var chain = new Chain(7, 25, CreateJoint);
            Add(chain);

            var head = chain.Joints[0];
            head.AddEffect(new Follow(Mouse) { Velocity = 1000, SlowRange = 100 });
        }

        private CompositeModel CreateJoint()
        {
            var joint = new CompositeModel();
            
            var center = new Circle(5);
            center.Style = Style.Clone();
            
            var outer = new Circle(25);
            outer.Style = Style.Clone();
            outer.Style.FillOpacity = 0;
            outer.Style.StrokeDashArray = "1%";

            joint.Add(center);
            joint.Add(outer);

            return joint;
        }
    }
}
