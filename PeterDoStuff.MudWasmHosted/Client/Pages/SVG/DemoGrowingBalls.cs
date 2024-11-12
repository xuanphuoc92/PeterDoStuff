using PeterDoStuff.Tools.Graphics;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.SVG
{
    public class DemoGrowingBalls : Canvas
    {
        public DemoGrowingBalls(ModelStyler builder) : base(300, 300, builder)
        {
            Name = "Growing Balls";

            double minSpeed = 50;
            double maxSpeed = 500;

            double speedRange = maxSpeed - minSpeed;

            Random random = new Random();

            for (int i = 0; i < 3; i++)
            {
                var ball = new Circle(10);
                ball.X = (random.NextDouble() * Width);
                ball.Y = (random.NextDouble() * Height);
                double velocityX = minSpeed + (random.NextDouble() * speedRange);
                double velocityY = minSpeed + (random.NextDouble() * speedRange);

                var growingAnimation = new HeartBeat(1, 3);
                growingAnimation.BeatPeriod = TimeSpan.FromSeconds(5);
                growingAnimation.GrowPhase = 0.5;

                var bouncingAnimation = new BouncingBall(0, Width, 0, Height, velocityX, velocityY);

                ball.AddAnimation(growingAnimation);
                ball.AddAnimation(bouncingAnimation);

                AddAndStyleModel(ball);
            }
        }
    }
}
