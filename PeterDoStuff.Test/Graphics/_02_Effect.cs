using FluentAssertions;
using PeterDoStuff.Graphics;
using PeterDoStuff.Graphics.Effects;
using PeterDoStuff.Graphics.Models;

namespace PeterDoStuff.Test.Graphics
{
    [TestClass]
    public class _02_Effect
    {
        [TestMethod]
        public void _01_ModelEffect()
        {
            var circle = new CircleModel(10);
            circle.X.Should().Be(0);
            circle.Y.Should().Be(0);

            circle.Apply(new StickTo(100, 200));
            circle.X.Should().Be(0);
            circle.Y.Should().Be(0);

            circle.Resolve();
            circle.X.Should().Be(100);
            circle.Y.Should().Be(200);
        }

        [TestMethod]
        public void _02_CanvasEffect()
        {
            var canvas = new CanvasModel(300, 300);

            var circle = new CircleModel(10);
            circle.Apply(new StickTo(100, 200));
            canvas.Add(circle);

            circle.X.Should().Be(0);
            circle.Y.Should().Be(0);

            canvas.Resolve();
            circle.X.Should().Be(100);
            circle.Y.Should().Be(200);
        }

        [TestMethod]
        public void _03_DisabledEffect()
        {
            var circle = new CircleModel(10);
            circle.X.Should().Be(0);
            circle.Y.Should().Be(0);

            circle.Apply(new StickTo(100, 200) { Enabled = false });
            circle.X.Should().Be(0);
            circle.Y.Should().Be(0);

            circle.Resolve();
            circle.X.Should().Be(0);
            circle.Y.Should().Be(0);
        }

        [TestMethod]
        public void _04_Moving()
        {
            var circle = new CircleModel(10);
            circle.X.Should().Be(0);
            circle.Y.Should().Be(0);

            circle.Apply(new Moving(1, 1));
            circle.Resolve();
            circle.X.Should().Be(0);
            circle.Y.Should().Be(0);

            circle.Resolve();
            circle.X.Should().BeGreaterThan(0);
            circle.Y.Should().BeGreaterThan(0);
            circle.X.Should().Be(circle.Y);
        }

        [TestMethod]
        public void _05_PointTo()
        {
            var model = new Model();
            var anchor = new Model();
            model.Apply(new PointTo(anchor));

            model.Deg.Should().Be(0);

            model.Resolve();
            model.Deg.Should().Be(0);

            (anchor.X, anchor.Y) = (1, 0);
            model.Resolve();
            model.Deg.Should().Be(0);

            (anchor.X, anchor.Y) = (1, 1);
            model.Resolve();
            model.Deg.Should().Be(45);

            (anchor.X, anchor.Y) = (0, 1);
            model.Resolve();
            model.Deg.Should().Be(90);

            (anchor.X, anchor.Y) = (1, -1);
            model.Resolve();
            model.Deg.Should().Be(-45);

            (anchor.X, anchor.Y) = (-1, -1);
            model.Resolve();
            model.Deg.Should().Be(-135);

            (anchor.X, anchor.Y) = (-1, 0);
            model.Resolve();
            model.Deg.Should().Be(-180);

            model.Effects.Clear();
            model.Apply(new PointTo(anchor, PointMode.Copy));
            model.Resolve();
            model.Deg.Should().Be(0);

            anchor.Deg = 45;
            model.Resolve();
            model.Deg.Should().Be(45);
        }

        [TestMethod]
        public async Task _06_Rotating()
        {
            var model = new Model();
            model.Apply(new Rotating(TimeSpan.FromMilliseconds(100)));
            
            model.Deg.Should().Be(0);
            model.Resolve();
            model.Deg.Should().Be(0);
            model.Resolve();
            model.Deg.Should().BeGreaterThan(0);
            await Task.Delay(50);
            model.Resolve();
            model.Deg.Should().BeLessThan(0);
        }

        [TestMethod]
        public void _07_ManualResolve()
        {
            var model = new Model();
            var stickTo = new StickTo(100, 200);
            stickTo.Resolve(model);
            model.X.Should().Be(100);
            model.Y.Should().Be(200);
        }

        [TestMethod]
        public void _08_MoveOffset()
        {
            var model = new Model();
            
            var anchor = new Model();
            anchor.X = 100;
            anchor.Y = 200;

            StickTo stickTo = new StickTo(anchor);
            stickTo.Offset.X = 1;
            stickTo.Offset.Y = 1;

            model.Apply(stickTo);
            model.Resolve();
            model.X.Should().Be(101);
            model.Y.Should().Be(201);

            anchor.Deg = 90;
            model.Apply(stickTo);
            model.Resolve();
            model.X.Should().Be(99);
            model.Y.Should().Be(201);

            anchor.Deg = 180;
            model.Apply(stickTo);
            model.Resolve();
            model.X.Should().Be(99);
            model.Y.Should().Be(199);
        }

        [TestMethod]
        public void _09_Follow()
        {
            var model = new Model();
            
            var anchor = new Model();
            anchor.X = 1000;
            anchor.Y = 1000;

            var follow = new Follow(anchor, 100);
            model.Apply(follow);
            
            // First tick, not moving
            model.Resolve();
            model.X.Should().Be(0);
            model.Y.Should().Be(0);
            model.Deg.Should().Be(45);

            // Second tick, moving
            model.Resolve();
            model.X.Should().BeGreaterThan(0);
            model.Y.Should().BeGreaterThan(0);
            model.Deg.Should().Be(45);

            // In merge range, merged
            model.X = 999;
            model.Y = 999;
            model.Resolve();
            model.X.Should().Be(1000);
            model.Y.Should().Be(1000);
            model.Deg.Should().Be(45);
        }
    }
}
