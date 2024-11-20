namespace PeterDoStuff.Graphics
{
    public abstract class Effect
    {
        public Model Model;

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

    public static class EffectExtensions
    {
        public static void Resolve(this Effect effect, Model model)
        {
            effect.Model = model;
            effect.Resolve(DateTime.Now);
        }
    }
}
