namespace PeterDoStuff.Graphics
{
    public abstract class Effect
    {
        public List<Model> Models = [];

        public bool Enabled = true;

        private DateTime? LastTick;
        protected TimeSpan TimeFromLastTick { get; private set; } = TimeSpan.Zero;

        public void Resolve(DateTime now)
        {
            if (Enabled == false)
                return;

            TimeFromLastTick = LastTick == null ? TimeSpan.Zero : now - LastTick.Value;
            Tick();
            LastTick = now;
        }

        public abstract void Tick();
    }
}
