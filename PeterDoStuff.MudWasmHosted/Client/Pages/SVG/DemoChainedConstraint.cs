using PeterDoStuff.Tools.Graphics;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.SVG
{
    public class DemoChainedConstraint : Canvas
    {
        public DemoChainedConstraint(Style? style = null) : base(300, 300, style)
        {
            Name = "Chained Constraint";

            Mouse = CreateJoint();
            var anchor = Mouse;

            for (int i = 0; i < 6; i++)
            {
                var joint = CreateJoint();
                joint.AddEffect(new DistanceConstraint(anchor, 25));
                Add(joint);
                anchor = joint;
            }
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

            joint.Models.Add(center);
            joint.Models.Add(outer);

            joint.AddEffect(j => center.MoveTo(j));
            joint.AddEffect(j => outer.MoveTo(j));
            return joint;
        }
    }
}
