using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlatPhysics
{

    public sealed class FlatWorld
    {
        private FlatVector gravity;
        private List<FlatBody> bodyList;
        private List<FlatManifold> contactList;
        public List<FlatVector> contactPointsList;

        public static readonly int MinIterations = 1;
        public static readonly int MaxIterations = 10;
        public int BodyCount
        {
            get { return bodyList.Count; }
        }

        public FlatWorld()
        {
            contactList = new List<FlatManifold>();
            gravity = new FlatVector(0, -9.81f);
            bodyList = new List<FlatBody>();
            contactPointsList = new List<FlatVector>();
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
            contactPointsList.Clear();
            for (int iter = 0; iter < iteration; iter++)
            {
            //Movement Step
                for (int i = 0; i < bodyList.Count; i++)
                {
                    bodyList[i].isColliding = false;
                    bodyList[i].Step(deltaTime, gravity, iteration);
                }

                contactList.Clear();
                //Collision Step
                for (int i = 0; i < bodyList.Count; i++)
                {
                    FlatBody bodyA = bodyList[i];
                    for (int j = i + 1; j < bodyList.Count; j++)
                    {
                        FlatBody bodyB = bodyList[j];
                        if(bodyA.IsStatic && bodyB.IsStatic)
                            continue;

                        if(Collisions.Collide(bodyA, bodyB, out FlatVector normal, out float depth))
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

                            Collisions.FindContactPoints(bodyA, bodyB, out FlatVector contact1, out FlatVector contact2, out int contactCount);
                            FlatManifold manifold = new FlatManifold(bodyA, bodyB, normal, depth, contact1, contact2, contactCount);
                            contactList.Add(manifold);

                            
                        }
                    }

                }
                for(int i = 0; i < contactList.Count; i++)
                {
                    FlatManifold manifold = contactList[i];
                    ResolveCollision(in manifold);//计算冲量
                    if(manifold.contactCount > 0)
                    {
                        contactPointsList.Add(manifold.contact1);   
                        if(manifold.contactCount > 1)
                            contactPointsList.Add(manifold.contact2);
                    }
                }
            }
        }

        public void ResolveCollision(in FlatManifold contact)
        {
            FlatBody bodyA = contact.bodyA;
            FlatBody bodyB = contact.bodyB;
            FlatVector normal = contact.normal;

            //Calculate impulse
            FlatVector relativeVelocity = bodyB.LinearVelocity - bodyA.LinearVelocity;
            if(FlatMath.Dot(relativeVelocity, normal) > 0)//如果b的速度比a的速度慢，则不发生碰撞
                return;

            float e = Mathf.Min(bodyA.Restitution, bodyB.Restitution);
            float j = -(1 + e) * FlatMath.Dot(relativeVelocity, normal) / (bodyA.InvMass + bodyB.InvMass);
            bodyA.LinearVelocity -= j * bodyA.InvMass * normal;
            bodyB.LinearVelocity += j * bodyB.InvMass * normal;
        }
        
        
    }
}
