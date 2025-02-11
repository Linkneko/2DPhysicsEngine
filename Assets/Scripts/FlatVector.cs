using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlatPhysics
{
    public readonly struct FlatVector
    {
        public readonly float x;
        public readonly float y;

        public static readonly FlatVector Zero = new FlatVector(0f, 0f);

        public FlatVector(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public static FlatVector operator +(FlatVector a, FlatVector b)
        {
            return new FlatVector(a.x + b.x, a.y + b.y);
        }

        public static FlatVector operator -(FlatVector a, FlatVector b)
        {
            return new FlatVector(a.x - b.x, a.y - b.y);
        }

        public static FlatVector operator -(FlatVector v)
        {
            return new FlatVector(-v.x, -v.y);
        }

        public static FlatVector operator *(FlatVector v, float s)
        {
            return new FlatVector(v.x * s, v.y * s);
        }

        public static FlatVector operator *(float s, FlatVector v)
        {
            return new FlatVector(v.x * s, v.y * s);
        }

        public static FlatVector operator /(FlatVector v, float s)
        {
            return new FlatVector(v.x / s, v.y / s);
        }

        internal static FlatVector Transform(FlatVector v, FlatTransform transform)
        {
            return new FlatVector(
                transform.Cos * v.x - transform.Sin * v.y + transform.PositionX,
                transform.Sin * v.x + transform.Cos * v.y + transform.PositionY);
        }

        public bool Equals(FlatVector other)
        {
            return this.x == other.x && this.y == other.y;
        }

        public override bool Equals(object obj)
        {
            if (obj is FlatVector other)
            {
                return this.Equals(other);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return new { this.x, this.y }.GetHashCode();
        }

        public override string ToString()
        {
            return $"X: {this.x}, Y: {this.y}";
        }
    }
}