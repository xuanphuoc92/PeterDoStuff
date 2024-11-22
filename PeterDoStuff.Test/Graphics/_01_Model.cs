using FluentAssertions;
using PeterDoStuff.Graphics;
using PeterDoStuff.Graphics.Models;

namespace PeterDoStuff.Test.Graphics
{
    [TestClass]
    public class _01_Model
    {
        [TestMethod]
        public void _01_DefaultModel()
        {
            var model = new Model();
            model.X.Should().Be(0);
            model.Y.Should().Be(0);
            model.Z.Should().Be(0);
            model.Deg.Should().Be(0);
            model.Scale.Should().Be(1);
            model.Effects.Should().BeEmpty();
        }

        [TestMethod]
        public void _02_DefaultStyle()
        {
            var style = new Style();
            
            style.StrokeColor.Should().Be("");
            style.StrokeWidth.Should().Be(1);
            style.StrokeOpacity.Should().Be(1);

            style.FillColor.Should().Be("");
            style.FillOpacity.Should().Be(0);
        }

        [TestMethod]
        public void _03_Canvas()
        {
            var canvas = new CanvasModel(600, 400);
            canvas.Rect.Width.Should().Be(600);
            canvas.Rect.Height.Should().Be(400);
            canvas.Rect.Style.StrokeWidth.Should().Be(1);

            var customStyle = new Style() { StrokeWidth = 2 };
            canvas = new CanvasModel(400, 600, customStyle);
            canvas.Rect.Width.Should().Be(400);
            canvas.Rect.Height.Should().Be(600);
            canvas.Rect.Style.StrokeWidth.Should().Be(2);

            canvas = new CanvasModel(400, 600, customStyle, new CircleModel(10));
            canvas.Mouse.Should().BeOfType(typeof(CircleModel));
        }

        [TestMethod]
        public void _04_Transform()
        {
            var circle = new CircleModel(10);
            circle.SvgTransform.Should().Be("translate(0,0) rotate(0) scale(1)");
        }

        [TestMethod]
        public void _05_Polygon()
        {
            var polygon = new PolygonModel(0, 0, 1, 0, 1, 1, 0, 1);
            polygon.Points.Should().HaveCount(4);
        }

        [TestMethod]
        public void _06_Curve()
        {
            var curve = new CurveModel();
            curve.Points.Should().BeEmpty();

            curve.CurveTo(new Model());
            curve.Points.Should().HaveCount(1);

            curve.CurveTo(new Model(), new Model());
            curve.Points.Should().HaveCount(2);

            curve.IsClosed.Should().BeFalse();
            curve.Close();
            curve.IsClosed.Should().BeTrue();
            curve.Points.Should().HaveCount(2);
        }

        [TestMethod]
        public void _07_Ellipse()
        {
            var ellipse = new EllipseModel(120, 60);
            ellipse.Rx.Should().Be(120);
            ellipse.Ry.Should().Be(60);
        }

        [TestMethod]
        public void _08_Path()
        {
            var path = new PathModel();
            path.MoveTo(new Model());
            path.CurveTo(new Model(), new Model(), new Model());
            path.Commands.Should().HaveCount(2);
        }
    }
}
