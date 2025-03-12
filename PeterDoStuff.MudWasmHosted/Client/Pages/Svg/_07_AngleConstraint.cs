using PeterDoStuff.Graphics;
using PeterDoStuff.Graphics.Effects;
using PeterDoStuff.Graphics.Models;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.Svg
{
    public class _07_AngleConstraint : CanvasModel
    {
        public _07_AngleConstraint(int width, int height, Style? style = null, Model? mouse = null) : base(width, height, style, mouse)
        {
            Name = "Angle Constraint";

            var spine = new Chain(50, 15, i => CreateJoint(i), 45);
            spine.Head.Apply(new Follow(Mouse, 250));
            Add(spine);
        }

        private CircleModel CreateJoint(int i)
        {
            var size = 16 - (i * 0.24);
            var joint = new CircleModel(size);
            joint.Style = Style.Clone();
            joint.Style.StrokeWidth = 2;
            //joint.Style.StrokeOpacity = 0.2;
            joint.Style.FillOpacity = 0;
            return joint;
        }
    }
}
