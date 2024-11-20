namespace PeterDoStuff.Graphics.Effects
{
    public class Moving(double vx, double vy) : Effect
    {
        public double Vx = vx, Vy = vy;

        public override void Tick()
        {
            var dx = Vx * TimeFromLastTick.TotalSeconds;
            var dy = Vy * TimeFromLastTick.TotalSeconds;

            Model.X += dx;
            Model.Y += dy;
        }
    }
}
