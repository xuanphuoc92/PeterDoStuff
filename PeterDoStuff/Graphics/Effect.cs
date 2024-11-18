namespace PeterDoStuff.Graphics
{
    public abstract class Effect
    {
        public List<Model> Models = [];

        public bool Enabled = true;        

        public void Resolve(DateTime now)
        {
            if (Enabled == false)
                return;

            Tick(now);
            UpdateTick(now);
        }

        private DateTime? LastTick;
        private void UpdateTick(DateTime now)
        {
            LastTick = now;
        }

        public abstract void Tick(DateTime now);
    }
}
