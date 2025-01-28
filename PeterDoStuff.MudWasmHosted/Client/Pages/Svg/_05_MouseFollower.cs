using PeterDoStuff.Graphics;
using PeterDoStuff.Graphics.Effects;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.Svg
{
    public class _05_MouseFollower : CanvasModel
    {
        public _05_MouseFollower(int width, int height, Style? style = null, Model? mouse = null) : base(width, height, style, mouse)
        {
            Name = "Mouse Follower";

            var arrow = new Arrow(15, 150, 150);

            var follow = new Follow(Mouse, 250);
            follow.SlowRange = 100;
            arrow.Apply(follow);

            AddAndStyle(arrow);
        }
    }
}
