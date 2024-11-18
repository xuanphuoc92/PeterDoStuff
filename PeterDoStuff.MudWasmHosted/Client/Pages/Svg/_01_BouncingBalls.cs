using PeterDoStuff.Graphics;
using PeterDoStuff.Graphics.Effects;
using PeterDoStuff.Graphics.Models;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.Svg
{
    public class _01_BouncingBalls : CanvasModel
    {
        public _01_BouncingBalls(Style? style = null, Model? mouse = null) : base(300, 300, style, mouse)
        {
            Name = "Bouncing Balls";

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
                Models.ForEach(m =>
                {
                    if (m.X < MinX || m.X > MaxX)
                        Moving.Vx *= -1;

                    if (m.Y < MinY || m.Y > MaxY)
                        Moving.Vy *= -1;

                    if (m.X < MinX)
                        m.X = MinX;

                    if (m.X > MaxX)
                        m.X = MaxX;

                    if (m.Y < MinY)
                        m.Y = MinY;

                    if (m.Y > MaxY)
                        m.Y = MaxY;
                });
            }
        }
    }
}
