using MudBlazor;
using PeterDoStuff.Graphics;
using PeterDoStuff.Graphics.Effects;
using PeterDoStuff.Graphics.Models;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.Svg
{
    public class _01_BouncingBalls : CanvasModel
    {
        public _01_BouncingBalls(int width, int height, Style? style = null, Model? mouse = null) : base(width, height, style, mouse)
        {
            Name = "Bouncing Balls";

            Mouse.Style.StrokeOpacity *= 0.2;
            Mouse.Style.FillOpacity *= 0.2;

            var random = new Random();
            for (int i = 0; i < 10; i++)
            {
                var ball = new CircleModel(10);
                var moving = new Moving(50 + random.NextDouble() * 450, 50 + random.NextDouble() * 450);
                var bouncing = new Bouncing(moving, ball.Radius, Rect.Width - ball.Radius, ball.Radius, Rect.Height - ball.Radius);
                
                ball.Apply(moving);
                ball.Apply(bouncing);

                AddAndStyle(ball);
            }
        }

        private class Bouncing(Moving moving, double minX, double maxX, double minY, double maxY) : Effect
        {
            private Moving Moving = moving;
            private double 
                MinX = minX,
                MaxX = maxX,
                MinY = minY,
                MaxY = maxY;

            public override void Tick()
            {
                if (Model.X < MinX || Model.X > MaxX)
                    Moving.Vx *= -1;

                if (Model.Y < MinY || Model.Y > MaxY)
                    Moving.Vy *= -1;

                if (Model.X < MinX)
                    Model.X = MinX;

                if (Model.X > MaxX)
                    Model.X = MaxX;

                if (Model.Y < MinY)
                    Model.Y = MinY;

                if (Model.Y > MaxY)
                    Model.Y = MaxY;
            }
        }
    }
}
