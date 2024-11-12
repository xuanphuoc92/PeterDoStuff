using MudBlazor;
using PeterDoStuff.Tools.Graphics;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.SVG
{
    public class DemoSimpleCircle : Canvas
    {
        public DemoSimpleCircle() : base(300, 300)
        {
            Name = "Simple Circle";

            Model.DEFAULT_STROKE_COLOR = Colors.DeepPurple.Accent3;
            Model.DEFAULT_STROKE_WIDTH = 4;

            CanvasRect.StrokeColor = Model.DEFAULT_STROKE_COLOR;
            CanvasRect.StrokeWidth = 4;

            var circle = new Circle(25) { X = Width / 2, Y = Height / 2 };
            circle.AddAnimation(new BouncingBall(0, Width, 0, Height, 250, 0));
            AddModel(circle);
        }
    }
}
