namespace PeterDoStuff.Tools.Graphics
{
    public class Rectangle(double width, double height) : Model
    {
        public double Width => Scale * width;
        public double Height => Scale * height;
    }
}
