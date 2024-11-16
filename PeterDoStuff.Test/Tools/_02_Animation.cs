using FluentAssertions;
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
            circle.Style.StrokeOpacity.Should().Be(1);
            circle.Style.FillOpacity.Should().Be(0);
            circle.SvgTransform.WriteToConsole();
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
            rect.AddEffect(moveRight);
            await rect.Resolve();
            rect.X.Should().Be(1);
            rect.ClearEffects();
            rect.AddEffect(r => r.X--); // Move left
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
            circle.AddEffect(new BouncingInBox(50, 450, 50, 50, 1, 0));
            canvas.AddAndStyle(circle);

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
            circle.AddEffect(new BouncingInBox(0, 100, 0, 100, 1, 1));

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
            circle.AddEffect(new BouncingBall(0, 500, 0, 500, 1, 1));

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
            circle.AddEffect(new HeartBeat(1, 1.2));

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
        public async Task _09_Clockwising()
        {
            var circle = new Circle(50);
            
            Clockwising animation = new Clockwising(300, 300, 100);
            animation.Period = TimeSpan.FromMilliseconds(100);
            circle.AddEffect(animation);

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
            circle.AddEffect(new Blink(0, 100, 0, 200)
            {
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
            circle.AddEffect(new Follow(anchor)
            {
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
        public void _13_Style()
        {
            var style = new Style().Clone().SetStroke("#FFFFFF", 4, 0.5).SetFill("#000000", 0.2);

            var canvas = new Canvas(300, 300, style);

            var circle = new Circle(5);
            canvas.AddAndStyle(circle);
            circle.Radius.Should().Be(5);

            circle.Style.StrokeColor.Should().Be("#FFFFFF");
            circle.Style.StrokeWidth.Should().Be(4);
            circle.Style.StrokeOpacity.Should().Be(0.5);

            circle.Style.FillColor.Should().Be("#000000");
            circle.Style.FillOpacity.Should().Be(0.2);
        }

        [TestMethod]
        public async Task _14_Wander()
        {
            var circle = new Circle(5);
            var wander = new Wander(0, 1000, 0, 1000);
            wander.Blink.BlinkGap = TimeSpan.FromMilliseconds(100);
            wander.Follow.Velocity = 250;
            wander.Follow.SlowRange = 200;
            circle.AddEffect(wander);

            wander.Blink.BlinkGap.Should().Be(TimeSpan.FromMilliseconds(100));
            wander.Follow.Velocity.Should().Be(250);
            wander.Follow.SlowRange.Should().Be(200);

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

            line.End.AddEffect(new Blink(3, 100, 4, 200)
            {
                BlinkGap = TimeSpan.FromMilliseconds(100)
            });

            await line.Resolve();
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
            
            path.Models.Should().HaveCount(1);
            path.Commands.Should().HaveCount(1);

            path.LineTo(100, 150);
            path.LineTo(150, 150);
            path.LineTo(200, 150);
            path.LineTo(250, 150);
            path.Models.Should().HaveCount(5);
            path.Commands.Should().HaveCount(5);
            path.ClosePath();
            path.Models.Should().HaveCount(5);
            path.Commands.Should().HaveCount(6);
        }

        [TestMethod]
        public void _17_Curve()
        {
            var curve = new PathModel(50, 150);

            curve.Models.Should().HaveCount(1);
            curve.Commands.Should().HaveCount(1);

            curve.QuadCurveTo(100, 150, 150, 150);
            curve.QuadCurveTo(200, 150, 250, 150);
            curve.Models.Should().HaveCount(5);
            curve.Commands.Should().HaveCount(3);

            curve = new PathModel(50, 150);

            curve.Models.Should().HaveCount(1);
            curve.Commands.Should().HaveCount(1);

            curve.QuadCurveTo(150, 150);
            curve.QuadCurveTo(250, 150);
            curve.Models.Should().HaveCount(3);
            curve.Commands.Should().HaveCount(3);
        }

        [TestMethod]
        public async Task _18_Polygon()
        {
            var polygon = new Polygon(0, -50, 25, 25, 0, 0, -25, 25);
            polygon.Points.Should().HaveCount(4);
            polygon.Models.Should().HaveCount(4);

            var rotating = new Rotating(TimeSpan.FromMilliseconds(100));
            polygon.AddEffect(rotating);
            polygon.Degrees.Should().Be(0);

            await polygon.Resolve();
            polygon.Degrees.Should().Be(0);

            await polygon.Resolve();
            polygon.Degrees.Should().BeGreaterThan(0);

            await Task.Delay(25);
            await polygon.Resolve();
            polygon.Degrees.Should().BeGreaterThan(45);

            await Task.Delay(75);
            await polygon.Resolve();
        }

        [TestMethod]
        public async Task _19_A_DistanceConstraint()
        {
            var point = new Model() { X = 0, Y = 0 };
            var anchor = new Model() { X = 0, Y = 0 };

            var distanceConstraint = new DistanceConstraint(anchor, 100);
            point.AddEffect(distanceConstraint);

            (anchor.X, anchor.Y) = (10, 10);
            await point.Resolve();
            point.X.Should().Be(0);
            point.Y.Should().Be(0);

            (anchor.X, anchor.Y) = (80, 80);
            await point.Resolve();
            point.X.Should().BeGreaterThan(0);
            point.Y.Should().BeGreaterThan(0);
        }

        [TestMethod]
        public async Task _19_B_DistanceConstraint()
        {
            var point = new Model() { X = 0, Y = 0 };
            var anchor = new Model() { X = 0, Y = 0 };

            var distanceConstraint = new DistanceConstraint(anchor, 100);
            distanceConstraint.MinDistance = 100;
            point.AddEffect(distanceConstraint);

            (anchor.X, anchor.Y) = (1, 1);
            await point.Resolve();
            point.X.Should().BeLessThan(0);
            point.Y.Should().BeLessThan(0);
        }

        [TestMethod]
        public async Task _20_ConstraintTickGap()
        {
            var blink = new Blink(3, 100, 4, 200);
            blink.ConstraintTickGap = TimeSpan.FromMilliseconds(100);
            blink.LastTick.Should().BeNull();
            
            await blink.Resolve(DateTime.Now);
            await Task.Delay(10);
            await blink.Resolve(DateTime.Now);
            await Task.Delay(120);
            await blink.Resolve(DateTime.Now);
        }

        [TestMethod]
        public void _21_PointTo()
        {
            var model = new Model();
            var anchor = new Model();
            model.Degrees.Should().Be(0);
            model.PointTo(anchor);
            model.Degrees.Should().Be(0);

            anchor.X = 1;
            model.PointTo(anchor);
            model.Degrees.Should().Be(90);
        }

        [TestMethod]
        public void _22_Chain()
        {
            var chain = new Chain(8, 90, () => new Circle(30));
            chain.Joints.Should().HaveCount(8);
        }
    }
}
