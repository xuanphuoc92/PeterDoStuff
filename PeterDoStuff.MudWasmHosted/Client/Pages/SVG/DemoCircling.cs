using PeterDoStuff.Tools.Graphics;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.SVG
{
    public class DemoCircling : Canvas
    {
        public DemoCircling() : base(300, 300)
        {
            Name = "Circling";
            DateTime now = DateTime.Now;
            
            var circle = new Circle(10);
            circle.StrokeWidth = 10;
            circle.AddAnimation(new Circling(Width /2 , Height / 2, 100) { LastTick = now });
            AddModel(circle);

            var text = new Text();
            text.X = Width / 2;
            text.Y = Height / 2 + 75 / 3;
            text.Content = "0";
            text.FontSize = 75;
            text.StrokeWidth = 0;
            text.FillColor = Model.DEFAULT_STROKE_COLOR;
            text.FillOpacity = 1;
            text.AddAnimation(new Timer() { LastTick = now });
            AddModel(text);
        }

        private class Timer : Animation
        {
            private TimeSpan TotalTimeSpan = TimeSpan.Zero;

            public async override Task Tick(TimeSpan? timeSpan = null)
            {
                TotalTimeSpan += timeSpan ?? TimeSpan.Zero;
                var textModel = (Text)Model;
                textModel.Content = ((int)TotalTimeSpan.TotalSeconds).ToString();
            }
        }
    }
}
