﻿namespace PeterDoStuff.Tools.Graphics
{
    public class Circle(double radius) : Model
    {
        public double Radius => Scale * radius;
    }
}