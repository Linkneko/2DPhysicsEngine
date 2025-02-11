using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlatPhysics
{
    public readonly struct FlatAABB
    {
        public readonly FlatVector Min;
        public readonly FlatVector Max;
        public FlatAABB(FlatVector min, FlatVector max)
        {
            Min = min;
            Max = max;
        }

        public FlatAABB(float minX, float minY, float maxX, float maxY)
        {
            Min = new FlatVector(minX, minY);
            Max = new FlatVector(maxX, maxY);
        }
    }
}
