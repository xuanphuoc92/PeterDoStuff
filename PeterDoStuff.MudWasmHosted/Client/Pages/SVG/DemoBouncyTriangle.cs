using PeterDoStuff.Tools.Graphics;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.SVG
{
    public class DemoBouncyTriangle : Canvas
    {
        public DemoBouncyTriangle(Style style) : base(300, 300, style)
        {
            Name = "Bouncy Triangle";

            double minSpeed = 50;
            double maxSpeed = 500;

            double speedRange = maxSpeed - minSpeed;

            Random random = new Random();

            var x1 = (random.NextDouble() * Width);
            var y1 = (random.NextDouble() * Height);
            var path = new PathModel(x1, y1);

            for (int i = 0; i < 2; i++)
            {
                var x = (random.NextDouble() * Width);
                var y = (random.NextDouble() * Height);
                path.LineTo(x, y);
            }

            for (int i = 0; i < 3; i++)
            {
                double velocityX = minSpeed + (random.NextDouble() * speedRange);
                double velocityY = minSpeed + (random.NextDouble() * speedRange);
                path.Models[i].AddEffect(new BouncingInBox(0, Width, 0, Height, velocityX, velocityY));
            }

            path.ClosePath();

            AddAndStyle(path);
        }
    }
}
