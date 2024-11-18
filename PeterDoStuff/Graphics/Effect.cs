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

            Tick();
        }

        public abstract void Tick();
    }
}
