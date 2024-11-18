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

            circle.Apply(new MoveTo(new Model() { X = 100, Y = 200 }));
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
            circle.Apply(new MoveTo(new Model() { X = 100, Y = 200 }));
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

            circle.Apply(new MoveTo(new Model() { X = 100, Y = 200 }) { Enabled = false });
            circle.X.Should().Be(0);
            circle.Y.Should().Be(0);

            circle.Resolve();
            circle.X.Should().Be(0);
            circle.Y.Should().Be(0);
        }
    }
}
