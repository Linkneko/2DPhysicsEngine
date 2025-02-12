using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlatPhysics
{

    public sealed class FlatWorld
    {
        private FlatVector gravity;
        private List<FlatBody> bodyList;


        public static readonly int MinIterations = 1;
        public static readonly int MaxIterations = 10;
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

        public void Step(float deltaTime, int iteration)
        {
            for(int iter = 0; iter < iteration; iter++)
            {
            //Movement Step
                for (int i = 0; i < bodyList.Count; i++)
                {
                    bodyList[i].isColliding = false;
                    bodyList[i].Step(deltaTime, gravity, iteration);
                }


                //Collision Step
                for (int i = 0; i < bodyList.Count; i++)
                {
                    FlatBody bodyA = bodyList[i];
                    for (int j = i + 1; j < bodyList.Count; j++)
                    {
                        FlatBody bodyB = bodyList[j];
                        if(bodyA.IsStatic && bodyB.IsStatic)
                            continue;

                        if(Collide(bodyA, bodyB, out FlatVector normal, out float depth))
                        {
                            if (bodyA.IsStatic)
                            {
                                bodyB.Move(normal * depth * 1f);
                            }
                            else if (bodyB.IsStatic)
                            {
                                bodyA.Move(-normal * depth * 1f);
                            }
                            else
                            {
                                bodyA.Move(-normal * depth * 0.5f);
                                bodyB.Move(normal * depth * 0.5f);
                            }//Move 作为位置修正，和运动学无关
                            bodyA.isColliding = true;
                            bodyB.isColliding = true;

                            ResolveCollision(bodyA, bodyB, normal, depth);//计算冲量
                        }
                    }

                }

            }
        }

        public void ResolveCollision(FlatBody bodyA, FlatBody bodyB, FlatVector normal, float depth)
        {
            FlatVector relativeVelocity = bodyB.LinearVelocity - bodyA.LinearVelocity;
            if(FlatMath.Dot(relativeVelocity, normal) > 0)//如果b的速度比a的速度慢，则不发生碰撞
                return;

            float e = Mathf.Min(bodyA.Restitution, bodyB.Restitution);
            float j = -(1 + e) * FlatMath.Dot(relativeVelocity, normal) / (bodyA.InvMass + bodyB.InvMass);
            bodyA.LinearVelocity -= j * bodyA.InvMass * normal;
            bodyB.LinearVelocity += j * bodyB.InvMass * normal;
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
                    return Collisions.IntersectPolygonPolygon(bodyA.GetTransformedVertices(), bodyB.GetTransformedVertices(),bodyA.Position ,bodyB.Position, out normal, out depth);
                }
            }

            return false;
        }
    }
}
