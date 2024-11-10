using FluentAssertions;
using PeterDoStuff.Extensions;
using PeterDoStuff.Test.Extensions;
using PeterDoStuff.Tools.Animation;

namespace PeterDoStuff.Test.Tools
{
    [TestClass]
    public class _02_Animation
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
    }
}
