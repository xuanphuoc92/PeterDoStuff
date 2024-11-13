namespace PeterDoStuff.Tools.Graphics
{
    public class Rectangle(double width, double height) : Model
    {
        public double Width = width, Height = height;
        public double ScaledWidth => Scale * Width;
        public double ScaledHeight => Scale * Height;
    }
}
