namespace PeterDoStuff.Tools.Graphics
{
    public class Circle(double radius) : Model
    {
        public double Radius = radius;
        public double ScaledRadius => Scale * Radius;
    }
}
