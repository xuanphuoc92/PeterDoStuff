namespace PeterDoStuff.Tools.Graphics
{
    public class BouncingBall : Animation
    {
        public BouncingBall(double Left, double Right, double Top, double Bottom, double VelocityX, double VelocityY)
        {
            (this.Left, this.Right, this.Top, this.Bottom, this.VelocityX, this.VelocityY) = (Left, Right, Top, Bottom, VelocityX, VelocityY);
            _bouncingInBox = new(Left, Right, Top, Bottom, VelocityX, VelocityY);
        }

        public double Left, Right, Top, Bottom, VelocityX, VelocityY;

        private BouncingInBox _bouncingInBox;

        protected override Task Resolve(DateTime now)
        {
            var circle = (Circle)Model;
            double ballRadius = circle.ScaledRadius + (circle.ScaledStrokeWidth / 2);
            _bouncingInBox.Model = Model;
            _bouncingInBox.Left = Left + ballRadius;
            _bouncingInBox.Right = Right - ballRadius;
            _bouncingInBox.Top = Top + ballRadius;
            _bouncingInBox.Bottom = Bottom - ballRadius;

            return _bouncingInBox.Tick(now);
        }
    }
}
