
namespace PeterDoStuff.Tools.Graphics
{
    public class Wander : Animation
    {
        private Model _anchor = new();
        private Blink _blink = new();
        private Follow _follow = new();
        
        public Wander()
        {
            _anchor.AddAnimation(_blink);
            _follow.Anchor = _anchor;
        }

        public TimeSpan Gap { get => _blink.BlinkGap; set => _blink.BlinkGap = value; }
        public double MinX { get => _blink.MinX; set => _blink.MinX = value; }
        public double MaxX { get => _blink.MaxX; set => _blink.MaxX = value; }
        public double MinY { get => _blink.MinY; set => _blink.MinY = value; }
        public double MaxY { get => _blink.MaxY; set => _blink.MaxY = value; }
        public double Velocity { get => _follow.Velocity; set => _follow.Velocity = value; }
        public double SlowRange { get => _follow.SlowRange; set => _follow.SlowRange = value; }

        public override async Task Tick(TimeSpan? timeSpan = null)
        {
            if (_anchor.X == default & _anchor.Y == default)
            {
                _anchor.X = Model.X; _anchor.Y = Model.Y;
            }

            if (_follow.Model == null)
                _follow.Model = Model;

            await _anchor.Tick(LastTick);
            await _follow.Tick(LastTick);
        }
    }
}
