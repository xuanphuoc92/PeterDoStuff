using PeterDoStuff.Tools.Graphics;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.SVG
{
    public class DemoTimer : Canvas
    {
        public DemoTimer(Style style) : base(300, 300, style)
        {
            Name = "Timer";
            DateTime now = DateTime.Now;
            
            var circle = new Circle(10);
            circle.AddEffect(new Clockwising(Width /2 , Height / 2, 100) { LastTick = now });
            AddAndStyle(circle);

            var text = new Text();
            text.X = Width / 2;
            text.Y = Height / 2 + 75 / 3;
            text.Content = "0";
            text.FontSize = 75;
            text.AddEffect(new Timer() { LastTick = now });
            AddAndStyle(text);
        }

        private class Timer : Effect
        {
            private TimeSpan TotalTimeSpan = TimeSpan.Zero;

            protected async override Task Resolve(DateTime now)
            {
                TotalTimeSpan += FromLastTick(now);
                var textModel = (Text)Model;
                textModel.Content = ((int)TotalTimeSpan.TotalSeconds).ToString();
            }
        }
    }
}
