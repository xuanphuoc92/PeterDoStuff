using FluentAssertions;
using PeterDoStuff.Extensions;
using PeterDoStuff.Test.Extensions;
using PeterDoStuff.Tools.Graphics;

namespace PeterDoStuff.Test.Tools
{
    [TestClass]
    public class _02_Graphics
    {
        [TestMethod]
        public void _01_Circle()
        {
            var circle = new Circle(5);
            circle.Radius.Should().Be(5);
            circle.X.Should().Be(0);
            circle.Y.Should().Be(0);
            circle.Z.Should().Be(0);
            circle.StrokeOpacity.Should().Be(1);
            circle.FillOpacity.Should().Be(0);
        }

        [TestMethod]
        public void _02_Rectangle()
        {
            var rect = new Rectangle(600, 400);
            rect.Width.Should().Be(600);
            rect.Height.Should().Be(400);
        }

        [TestMethod]
        public async Task _03_Tick()
        {
            var rect = new Rectangle(600, 400);
            await rect.Resolve();
            rect.X.Should().Be(0);
            rect.Y.Should().Be(0);

            Action<Model> moveRight = (model) => model.X++;
            rect.AddAnimation(moveRight);
            await rect.Resolve();
            rect.X.Should().Be(1);
            rect.ClearAnimations();
            rect.AddAnimation(r => r.X--); // Move left
            await rect.Resolve();
            rect.X.Should().Be(0);
        }

        //[TestMethod]
        //public async Task _04_PingPongX()
        //{
        //    var circle = new Circle(50);            
        //    circle.SetAnimation(new PingPongX(50, 100));
            
        //    circle.X.Should().Be(0);
        //    await circle.Tick();
        //    circle.X.Should().Be(51);
        //    await circle.Tick();
        //    circle.X.Should().Be(52);

        //    circle.X = 100;
        //    await circle.Tick();
        //    circle.X.Should().Be(99);
        //    await circle.Tick();
        //    circle.X.Should().Be(98);
        //}

        [TestMethod]
        public async Task _05_Canvas()
        {
            var canvas = new Canvas(500, 100);
            canvas.Width.Should().Be(500);
            canvas.Height.Should().Be(100);

            var circle = new Circle(50) { X = 50, Y = 50 };
            circle.AddAnimation(new BouncingInBox(50, 450, 50, 50, 1, 0));
            canvas.AddAndStyleModel(circle);

            circle.X.Should().Be(50);
            await canvas.Resolve();
            circle.X.Should().Be(50);
            await canvas.Resolve();
            circle.X.Should().BeGreaterThan(50);

            circle.X = 450;
            await canvas.Resolve();
            var prevX = circle.X;
            prevX.Should().Be(450);
            await canvas.Resolve();
            circle.X.Should().BeLessThan(prevX);
        }

        [TestMethod]
        public async Task _06_BouncingInBox()
        {
            var circle = new Circle(50);
            circle.AddAnimation(new BouncingInBox(0, 100, 0, 100, 1, 1));

            circle.X = -5;
            circle.Y = -5;
            await circle.Resolve();
            circle.X.Should().Be(0);
            circle.Y.Should().Be(0);

            circle.X = 105;
            circle.Y = 105;
            await circle.Resolve(DateTime.Now);
            circle.X.Should().Be(100);
            circle.Y.Should().Be(100);
        }

        [TestMethod]
        public async Task _07_BouncingBall()
        {
            var circle = new Circle(50);
            circle.AddAnimation(new BouncingBall(0, 500, 0, 500, 1, 1));

            circle.X = 5;
            circle.Y = 5;
            await circle.Resolve();
            circle.X.Should().Be(50.5);
            circle.Y.Should().Be(50.5);

            circle.X = 495;
            circle.Y = 495;
            await circle.Resolve(DateTime.Now);
            circle.X.Should().Be(449.5);
            circle.Y.Should().Be(449.5);
        }

        [TestMethod]
        public async Task _08_HeartBeat()
        {
            var circle = new Circle(50);
            circle.AddAnimation(new HeartBeat(1, 1.2));

            circle.Scale.Should().Be(1);
            await circle.Resolve(DateTime.Now);
            circle.Scale.Should().Be(1);
            await circle.Resolve(DateTime.Now);
            circle.Scale.Should().BeGreaterThan(1);
            await Task.Delay(210);
            await circle.Resolve(DateTime.Now);
            circle.Scale.Should().BeGreaterThan(1);
            circle.Scale.Should().BeLessThan(1.2);
            await Task.Delay(810);
            await circle.Resolve(DateTime.Now);
            circle.Scale.Should().BeGreaterThan(1);
        }

        [TestMethod]
        public async Task _09_Circling()
        {
            var circle = new Circle(50);
            
            Circling animation = new Circling(300, 300, 100);
            animation.CirclingPeriod = TimeSpan.FromMilliseconds(100);
            circle.AddAnimation(animation);

            await circle.Resolve(DateTime.Now);
            circle.X.Should().Be(300);
            circle.Y.Should().Be(200);

            await Task.Delay(25); // At around 3 o'clock
            await circle.Resolve(DateTime.Now);
            circle.X.Should().BeGreaterThan(300);
            circle.Y.Should().BeGreaterThan(200);

            await Task.Delay(85); // Back to 12 o'clock
            await circle.Resolve(DateTime.Now);
            circle.X.Should().BeGreaterThanOrEqualTo(300);
            circle.Y.Should().BeGreaterThanOrEqualTo(200);
        }

        [TestMethod]
        public void _10_Text()
        {
            var text = new Text();
            text.Content = "Hello";
            text.Content.Should().Be("Hello");
        }

        [TestMethod]
        public async Task _11_Blink()
        {
            var circle = new Circle(10);
            circle.AddAnimation(new Blink()
            {
                MinX = 0, MaxX = 100, MinY = 0, MaxY = 200,
                BlinkGap = TimeSpan.FromMilliseconds(100)
            });

            circle.X.Should().Be(0);
            circle.Y.Should().Be(0);

            await circle.Resolve(DateTime.Now);
            circle.X.Should().Be(0);
            circle.Y.Should().Be(0);

            await Task.Delay(101);
            await circle.Resolve(DateTime.Now);
            circle.X.Should().BeGreaterThan(0);
            circle.X.Should().BeLessThan(100);
            circle.Y.Should().BeGreaterThan(0);
            circle.Y.Should().BeLessThan(200);
        }

        [TestMethod]
        public async Task _12_Follow()
        {
            var anchor = new Circle(10);
            
            var circle = new Circle(10);
            circle.AddAnimation(new Follow()
            {
                Anchor = anchor,
                Velocity = 10,
            });

            circle.X.Should().Be(0);
            circle.Y.Should().Be(0);

            await circle.Resolve(DateTime.Now);
            circle.X.Should().Be(0);
            circle.Y.Should().Be(0);

            anchor.X = 1000;
            anchor.Y = 1000;

            await circle.Resolve(DateTime.Now);
            circle.X.Should().BeGreaterThan(0);
            circle.X.Should().BeLessThan(1000);
            circle.Y.Should().BeGreaterThan(0);
            circle.Y.Should().BeLessThan(1000);
            circle.X.Should().Be(circle.Y);
        }

        [TestMethod]
        public void _13_ModelBuilder()
        {
            var modelBuilder = new ModelStyler();
            modelBuilder.SetStroke("#FFFFFF", 4, 0.5).SetFill("#000000", 0.2);

            var canvas = new Canvas(300, 300, modelBuilder);

            var circle = new Circle(5);
            canvas.AddAndStyleModel(circle);
            circle.Radius.Should().Be(5);

            circle.StrokeColor.Should().Be("#FFFFFF");
            circle.StrokeWidth.Should().Be(4);
            circle.StrokeOpacity.Should().Be(0.5);

            circle.FillColor.Should().Be("#000000");
            circle.FillOpacity.Should().Be(0.2);
        }

        [TestMethod]
        public async Task _14_Wander()
        {
            var circle = new Circle(5);
            var wander = new Wander()
            {
                Gap = TimeSpan.FromMilliseconds(100),
                MinX = 0, MaxX = 1000,
                MinY = 0, MaxY = 1000,
                Velocity = 250,
                SlowRange = 200
            };
            circle.AddAnimation(wander);

            wander.Gap.Should().Be(TimeSpan.FromMilliseconds(100));
            wander.MinX.Should().Be(0);
            wander.MaxX.Should().Be(1000);
            wander.MinY.Should().Be(0);
            wander.MaxY.Should().Be(1000);
            wander.Velocity.Should().Be(250);
            wander.SlowRange.Should().Be(200);

            await circle.Resolve(DateTime.Now);
            circle.X.Should().Be(0);
            circle.Y.Should().Be(0);

            await Task.Delay(50);
            await circle.Resolve(DateTime.Now);
            circle.X.Should().Be(0);
            circle.Y.Should().Be(0);

            await Task.Delay(60);
            await circle.Resolve(DateTime.Now);
            circle.X.Should().BeGreaterThan(0);
            circle.Y.Should().BeGreaterThan(0);
        }

        [TestMethod]
        public async Task _15_Line()
        {
            var line = new Line().Set(1, 2, 3, 4);
            line.Start.X.Should().Be(1);
            line.Start.Y.Should().Be(2);
            line.End.X.Should().Be(3);
            line.End.Y.Should().Be(4);

            line.End.AddAnimation(new Blink()
            {
                MinX = 3,
                MaxX = 100,
                MinY = 4,
                MaxY = 200,
                BlinkGap = TimeSpan.FromMilliseconds(100)
            });

            await line.Resolve(DateTime.Now);
            line.Start.X.Should().Be(1);
            line.Start.Y.Should().Be(2);
            line.End.X.Should().Be(3);
            line.End.Y.Should().Be(4);

            await Task.Delay(100);
            await line.Resolve(DateTime.Now);
            line.Start.X.Should().Be(1);
            line.Start.Y.Should().Be(2);
            line.End.X.Should().BeGreaterThanOrEqualTo(3);
            line.End.X.Should().BeLessThanOrEqualTo(100);
            line.End.Y.Should().BeGreaterThanOrEqualTo(4);
            line.End.Y.Should().BeLessThanOrEqualTo(200);
        }

        [TestMethod]
        public void _16_Path()
        {
            var path = new PathModel(50, 150);
            
            path.Children.Should().HaveCount(1);
            path.Commands.Should().HaveCount(1);

            path.LineTo(100, 150);
            path.LineTo(150, 150);
            path.LineTo(200, 150);
            path.LineTo(250, 150);
            path.Children.Should().HaveCount(5);
            path.Commands.Should().HaveCount(5);
        }

        [TestMethod]
        public void _17_Curve()
        {
            var curve = new PathModel(50, 150);

            curve.Children.Should().HaveCount(1);
            curve.Commands.Should().HaveCount(1);

            curve.SmoothCurveTo(150, 150, 100, 150);
            curve.SmoothCurveTo(250, 150, 200, 150);
            curve.Children.Should().HaveCount(5);
            curve.Commands.Should().HaveCount(3);
        }
    }
}
