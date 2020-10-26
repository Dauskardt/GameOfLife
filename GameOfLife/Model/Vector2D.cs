using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace GameOfLife.Model
{
    public class Vector2D
    {
        public float X { get; set; }
        public float Y { get; set; }

        public Vector2D() { }

        public Vector2D(float x, float y) { X = x; Y = y; }

        public bool Equal(Vector2D TestVector)
        {
            return TestVector.X == X && TestVector.Y == Y ;
        }

        public Vector2D Clone()
        {
            return new Vector2D(X, Y);
        }

        public override string ToString()
        {
            return "Vector x:" + X + " y:" + Y;
        }
        

        #region Methodes..

        public float mag()
        {
            return (float)Math.Sqrt(X * X + Y * Y);
        }

        public Vector2D norm()
        {
            float r = 1/mag();
            return new Vector2D(X*r,Y*r);
        }
        
        public Vector2D perp()
        {
            return new Vector2D(-Y, X);
        }

        #endregion

        #region Operators..

        public static Vector2D operator +(Vector2D a, Vector2D b)
        => new Vector2D(a.X + b.X, a.Y + b.Y);

        public static Vector2D operator -(Vector2D a, Vector2D b)
        => new Vector2D(a.X - b.X, a.Y - b.Y);

        public static Vector2D operator *(Vector2D a, Vector2D b)
        => new Vector2D(a.X * b.X, a.Y * b.Y);

        public static Vector2D operator /(Vector2D a, Vector2D b)
        => new Vector2D(a.X / b.X, a.Y / b.Y);

        #endregion
    }
}
