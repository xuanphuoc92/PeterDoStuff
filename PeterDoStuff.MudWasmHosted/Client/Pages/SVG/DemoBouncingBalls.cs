using PeterDoStuff.Tools.Graphics;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.SVG
{
    public class DemoBouncingBalls : Canvas
    {
        public DemoBouncingBalls() : base(300, 300)
        {
            Name = "Bouncing Balls";
            
            double radius = 10;
            double ballStroke = 10;
            double minSpeed = 50;
            double maxSpeed = 500;

            double speedRange = maxSpeed - minSpeed;

            CanvasRect.StrokeWidth = 4;
            Random random = new Random();

            for (int i = 0; i < 10; i++)
            {
                var ball = new Circle(radius);
                ball.StrokeWidth = ballStroke;
                ball.X = (random.NextDouble() * Width);
                ball.Y = (random.NextDouble() * Height);
                double velocityX = minSpeed + (random.NextDouble() * speedRange);
                double velocityY = minSpeed + (random.NextDouble() * speedRange);
                var animation = new BouncingBall(0, Width, 0, Height, velocityX, velocityY);
                ball.AddAnimation(animation);
                AddModel(ball);
            }
        }
    }
}
