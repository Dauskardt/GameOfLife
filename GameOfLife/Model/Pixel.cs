using System;
using System.Collections.Generic;
using System.Text;

namespace GameOfLife.Model
{
    public class Pixel
    {
        public Vector2D Location { get; set; } = new Vector2D();
        public int State { get; set; } = 0;
        public Pixel() { }
        public Pixel(int state) 
        {
            State = state;
        }
        public Pixel(Vector2D location) 
        {
            Location = location;
        }
        public Pixel(Vector2D location, int state)
        {
            Location = location;
            State = state;
        }

        public override string ToString()
        {
            return Location.ToString();
        }
    }
}
