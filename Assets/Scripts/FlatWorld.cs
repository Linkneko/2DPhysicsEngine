using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlatPhysics
{

    public sealed class FlatWorld
    {
        private FlatVector gravity;
        private List<FlatBody> bodyList;

        public int BodyCount
        {
            get { return bodyList.Count; }
        }

        public FlatWorld()
        {
            gravity = new FlatVector(0, -9.81f);
            bodyList = new List<FlatBody>();
        }

        public void AddBody(FlatBody body)
        {
            bodyList.Add(body);
        }

        public void RemoveBody(FlatBody body)
        {
            bodyList.Remove(body);
        }

        public bool GetBody(int index, out FlatBody body)
        {
            body = null;
            if (index < 0 || index >= bodyList.Count)
            {
                return false;
            }
            body = bodyList[index];
            return true;
        }

        public void Step(float deltaTime)
        {
            //Movement Step
            for (int i = 0; i < bodyList.Count; i++)
            {
                bodyList[i].isColliding = false;
                bodyList[i].Step(deltaTime);
            }


            //Collision Step
            for (int i = 0; i < bodyList.Count; i++)
            {
                FlatBody bodyA = bodyList[i];
                for (int j = i + 1; j < bodyList.Count; j++)
                {
                    FlatBody bodyB = bodyList[j];
                    if(Collide(bodyA, bodyB, out FlatVector normal, out float depth))
                    {
                        bodyA.Move(-normal * depth * 0.5f);
                        bodyB.Move(normal * depth * 0.5f);
                        bodyA.isColliding = true;
                        bodyB.isColliding = true;
                    }
                }

            }
        }
        private bool Collide(FlatBody bodyA, FlatBody bodyB, out FlatVector normal, out float depth)
        {
            normal = FlatVector.Zero;
            depth = 0;

            if(bodyA.ShapeType == ShapeType.Circle)
            {
                if(bodyB.ShapeType == ShapeType.Circle)
                {
                    return Collisions.IntersectCircleCircle(bodyA.Position, bodyA.Radius, bodyB.Position, bodyB.Radius, out normal, out depth);
                }
                else if(bodyB.ShapeType == ShapeType.Box)
                {
                    bool result = Collisions.IntersectCirclePolygon(bodyA.Position, bodyA.Radius, bodyB.GetTransformedVertices(), out normal, out depth);
                    normal = -normal;
                    return result;
                }
            }else if(bodyA.ShapeType == ShapeType.Box)
            {
                if(bodyB.ShapeType == ShapeType.Circle)
                {
                    return Collisions.IntersectCirclePolygon(bodyB.Position, bodyB.Radius, bodyA.GetTransformedVertices(), out normal, out depth);
                }
                else if(bodyB.ShapeType == ShapeType.Box)
                {
                    return Collisions.IntersectPolygonPolygon(bodyA.GetTransformedVertices(), bodyB.GetTransformedVertices(), out normal, out depth);
                }
            }

            return false;
        }
    }
}
