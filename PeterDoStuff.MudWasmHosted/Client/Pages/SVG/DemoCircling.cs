using PeterDoStuff.Tools.Graphics;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.SVG
{
    public class DemoCircling : Canvas
    {
        public DemoCircling() : base(300, 300)
        {
            Name = "Circling";
            
            var circle = new Circle(10);
            circle.StrokeWidth = 10;
            circle.AddAnimation(new Circling(Width /2 , Height / 2, 100));
            AddModel(circle);
        }
    }
}
