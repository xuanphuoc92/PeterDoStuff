using PeterDoStuff.Graphics;
using PeterDoStuff.Graphics.Models;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.Svg
{
    public class _07_Snake : CanvasModel
    {
        public _07_Snake(Style? style = null, Model? mouse = null) : base(300, 300, style, mouse)
        {
            Name = "Snake";

            var spine = new Chain(24, 15, _ => CreateJoint(), 45);
            spine.Head.Apply(new Follow(Mouse, 250) { SlowRange = 50, StopRange = 5, MergeRange = 2 });
            Add(spine);
        }

        private CircleModel CreateJoint()
        {
            var joint = new CircleModel(16);
            joint.Style = Style.Clone();
            joint.Style.StrokeWidth = 2;
            //joint.Style.StrokeOpacity = 0.2;
            joint.Style.FillOpacity = 0;
            return joint;
        }
    }
}
