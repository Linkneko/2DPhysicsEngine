using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlatPhysics
{
    public readonly struct FlatManifold
    {
        public readonly FlatBody bodyA;
        public readonly FlatBody bodyB;
        public readonly FlatVector normal;// normal of the contact
        public readonly float depth;

        public readonly FlatVector contact1;
        public readonly FlatVector contact2;
        public readonly int contactCount;// number of contacts

        public FlatManifold(
            FlatBody bodyA, FlatBody bodyB, 
            FlatVector normal, float depth, 
            FlatVector contact1, FlatVector contact2, 
            int contactCount)
        {
            this.bodyA = bodyA;
            this.bodyB = bodyB;
            this.normal = normal;
            this.depth = depth;
            this.contact1 = contact1;
            this.contact2 = contact2;
            this.contactCount = contactCount;
        }
    }
}
