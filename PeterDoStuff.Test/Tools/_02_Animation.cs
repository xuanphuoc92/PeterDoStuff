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
            circle.StrokeColor.Should().Be(Model.DEFAULT_STROKE_COLOR);            
            circle.ToJson(beautify: true).WriteToConsole();

            var json = circle.ToJson();
            circle = json.FromJson<Circle>();
            circle.Radius.Should().Be(5);
        }

        [TestMethod]
        public void _02_Rectangle()
        {
            var rect = new Rectangle(600, 400);
            rect.Width.Should().Be(600);
            rect.Height.Should().Be(400);
            rect.ToJson(beautify: true).WriteToConsole();
        }

        [TestMethod]
        public async Task _03_Tick()
        {
            var rect = new Rectangle(600, 400);
            await rect.Tick();
            rect.X.Should().Be(0);
            rect.Y.Should().Be(0);

            Action<Model> moveRight = (model) => model.X++;
            rect.SetAnimation(moveRight);
            await rect.Tick();
            rect.X.Should().Be(1);
            rect.SetAnimation(r => r.X--); // Move left
            await rect.Tick();
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
            canvas.Width = 400;
            canvas.Height = 200;
            canvas.Width.Should().Be(400);
            canvas.Height.Should().Be(200);

            var circle = new Circle(50) { X = 50, Y = 50 };
            circle.SetAnimation(new BouncingInBox(50, 450, 50, 50, 1, 0));
            canvas.AddModel(circle);

            circle.X.Should().Be(50);
            await canvas.Tick();
            circle.X.Should().Be(50);
            await canvas.Tick();
            circle.X.Should().BeGreaterThan(50);

            circle.X = 450;
            await canvas.Tick();
            var prevX = circle.X;
            prevX.Should().Be(450);
            await canvas.Tick();
            circle.X.Should().BeLessThan(prevX);
        }

        [TestMethod]
        public async Task _06_BouncingInBox()
        {
            var circle = new Circle(50);
            circle.SetAnimation(new BouncingInBox(0, 100, 0, 100, 1, 1));

            circle.X = -5;
            circle.Y = -5;
            await circle.Tick();
            circle.X.Should().Be(0);
            circle.Y.Should().Be(0);

            circle.X = 105;
            circle.Y = 105;
            await circle.Tick(DateTime.Now);
            circle.X.Should().Be(100);
            circle.Y.Should().Be(100);
        }
    }
}
