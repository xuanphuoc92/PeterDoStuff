﻿using PeterDoStuff.Extensions;

namespace PeterDoStuff.Graphics.Effects
{
    public class Rotating : Effect
    {
        public Rotating(TimeSpan cycle) : this(360 / cycle.TotalSeconds)
        {
        }

        public Rotating(double degPerSec)
        {
            DegPerSec = degPerSec;
        }
        
        public double DegPerSec;

        public override void Tick()
        {
            var dDeg = DegPerSec * TimeFromLastTick.TotalSeconds;
            Models.ForEach(m =>
            {
                m.Deg = (m.Deg + dDeg).Cap(-180, 180);
            });
        }
    }
}