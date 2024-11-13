using PeterDoStuff.Tools.Graphics;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.SVG
{
    public class DemoCirclingTimer : Canvas
    {
        public DemoCirclingTimer(ModelStyler builder) : base(300, 300, builder)
        {
            Name = "Circling Timer";
            DateTime now = DateTime.Now;
            
            var circle = new Circle(10);
            circle.AddAnimation(new Circling(Width /2 , Height / 2, 100) { LastTick = now });
            AddAndStyleModel(circle);

            var text = new Text();
            text.X = Width / 2;
            text.Y = Height / 2 + 75 / 3;
            text.Content = "0";
            text.FontSize = 75;
            text.AddAnimation(new Timer() { LastTick = now });
            AddAndStyleModel(text);

            var line = new Line().Set(Width / 2, Height / 2, Width / 2, 75);
            AddAndStyleModel(line);
            line.StrokeOpacity = 0.75;
            line.End.AddAnimation(new Circling(Width / 2, Height / 2, 75) { LastTick = now });
        }

        private class Timer : Animation
        {
            private TimeSpan TotalTimeSpan = TimeSpan.Zero;

            public async override Task Resolve(DateTime now)
            {
                TotalTimeSpan += FromLastTick(now);
                var textModel = (Text)Model;
                textModel.Content = ((int)TotalTimeSpan.TotalSeconds).ToString();
            }
        }
    }
}
