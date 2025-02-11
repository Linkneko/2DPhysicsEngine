using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace FlatPhysics
{
    public static class FlatMath
    {
        public static float Clamp(float value, float min, float max)
        {
            if (min == max)
            {
                return min;
            }

            if (min > max)
            {
                throw new ArgumentOutOfRangeException("min is greater than the max.");
            }

            if (value < min)
            {
                return min;
            }

            if (value > max)
            {
                return max;
            }

            return value;
        }



        public static float Length(FlatVector v)
        {
            return MathF.Sqrt(v.x * v.x + v.y * v.y);
        }

        public static float Distance(FlatVector a, FlatVector b)
        {
            return FlatMath.Length(a - b);
        }

        public static FlatVector Normalize(FlatVector v)
        {
            float len = FlatMath.Length(v);
            return new FlatVector(v.x / len, v.y / len);
        }

        public static float Dot(FlatVector a, FlatVector b)
        {
            return a.x * b.x + a.y * b.y;
        }

        public static float Cross(FlatVector a, FlatVector b)
        {
            return a.x * b.y - a.y * b.x;
        }

    }
}