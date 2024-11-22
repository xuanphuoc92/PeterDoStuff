using PeterDoStuff.Graphics;
using PeterDoStuff.Graphics.Effects;
using PeterDoStuff.Graphics.Models;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.Svg
{
    public class _07_AngleConstraint : CanvasModel
    {
        public _07_AngleConstraint(Style? style = null, Model? mouse = null) : base(300, 300, style, mouse)
        {
            Name = "Angle Constraint";

            var spine = new Chain(24, 15, i => CreateJoint(i), 45);
            spine.Head.Apply(new Follow(Mouse, 250));
            Add(spine);
        }

        private CircleModel CreateJoint(int i)
        {
            var joint = new CircleModel(16 - i*0.5);
            joint.Style = Style.Clone();
            joint.Style.StrokeWidth = 2;
            //joint.Style.StrokeOpacity = 0.2;
            joint.Style.FillOpacity = 0;
            return joint;
        }
    }
}
