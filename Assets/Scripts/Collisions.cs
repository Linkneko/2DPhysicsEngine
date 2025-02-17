using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlatPhysics
{
    public static class Collisions
    {
        public static bool IntersectsAABBs(FlatAABB aabbA, FlatAABB aabbB)
        {
            if(aabbA.Max.x < aabbB.Min.x || aabbB.Max.x < aabbA.Min.x || aabbA.Max.y < aabbB.Min.y || aabbB.Max.y < aabbA.Min.y)
                return false;
            return true;
        
        }
        public static bool Collide(FlatBody bodyA, FlatBody bodyB, out FlatVector normal, out float depth)
        {
            normal = FlatVector.Zero;
            depth = 0;

            if (bodyA.ShapeType == ShapeType.Circle)
            {
                if (bodyB.ShapeType == ShapeType.Circle)
                {
                    return IntersectCircleCircle(bodyA.Position, bodyA.Radius, bodyB.Position, bodyB.Radius, out normal, out depth);
                }
                else if (bodyB.ShapeType == ShapeType.Box)
                {
                    bool result = IntersectCirclePolygon(bodyA.Position, bodyA.Radius, bodyB.GetTransformedVertices(), out normal, out depth);
                    normal = -normal;
                    return result;
                }
            }
            else if (bodyA.ShapeType == ShapeType.Box)
            {
                if (bodyB.ShapeType == ShapeType.Circle)
                {
                    return IntersectCirclePolygon(bodyB.Position, bodyB.Radius, bodyA.GetTransformedVertices(), out normal, out depth);
                }
                else if (bodyB.ShapeType == ShapeType.Box)
                {
                    return IntersectPolygonPolygon(bodyA.GetTransformedVertices(), bodyB.GetTransformedVertices(), bodyA.Position, bodyB.Position, out normal, out depth);
                }
            }

            return false;
        }
        public static void FindContactPoints(
            FlatBody bodyA, FlatBody bodyB,
            out FlatVector contact1, out FlatVector contact2,
            out int contactCount)
        {
            contact1 = FlatVector.Zero;
            contact2 = FlatVector.Zero;
            contactCount = 0;

            if (bodyA.ShapeType == ShapeType.Circle)
            {
                if (bodyB.ShapeType == ShapeType.Circle)
                {
                    FindContactPoint(bodyA.Position, bodyA.Radius, bodyB.Position, out contact1);
                    contactCount = 1;
                }
                else if (bodyB.ShapeType == ShapeType.Box)
                {
                    
                }
            }
            else if (bodyA.ShapeType == ShapeType.Box)
            {
                if (bodyB.ShapeType == ShapeType.Circle)
                {

                }
                else if (bodyB.ShapeType == ShapeType.Box)
                {

                }
            }
        } 
        
        private static void FindContactPoint(FlatVector centerA, float radiusA, FlatVector centerB, out FlatVector cp)
        {
            FlatVector direction = centerB - centerA;
            direction = FlatMath.Normalize(direction);
            cp = centerA + direction * radiusA;
        }
        public static bool IntersectCirclePolygon(FlatVector center, float radius
            , FlatVector[] vertices, out FlatVector normal, out float depth)
        {
            normal = FlatVector.Zero;
            depth = float.MaxValue;
            float minA, maxA, minB, maxB;
            float axisDepth;
            bool flag = false;
            for (int i = 0; i < vertices.Length; i++)
            {
                FlatVector va = vertices[i];
                FlatVector vb = vertices[(i + 1) % vertices.Length];

                FlatVector eage = vb - va;
                FlatVector axis = FlatMath.Normalize(new FlatVector(-eage.y, eage.x));

                ProjectVertices(vertices, axis, out minA, out maxA);
                ProjectCirlce(center, radius, axis, out minB, out maxB);
                if (maxA < minB || maxB < minA)  
                {
                    return false;
                }


                axisDepth = Mathf.Min(maxA - minB, maxB - minA);
                if (axisDepth < depth)
                {
                    normal = axis;
                    depth = axisDepth;
                    flag = (maxA - minB) > (maxB - minA);
                }
            }

            int cpIndex = FindClosestVertex(vertices, center);
            FlatVector cp = vertices[cpIndex];
            FlatVector axis1 = FlatMath.Normalize(cp - center);

            ProjectVertices(vertices, axis1, out minA, out maxA);
            ProjectCirlce(center, radius, axis1, out minB, out maxB);
            if (maxA < minB || maxB < minA)  
            {
                return false;
            }


            axisDepth = Mathf.Min(maxA - minB, maxB - minA);
            if (axisDepth < depth)
            {
                normal = axis1;
                depth = axisDepth;
                flag = (maxA - minB) > (maxB - minA);
            }
            if (flag)
            {
                normal = normal * -1;
            }
            return true;
        }
        private static int FindClosestVertex(FlatVector[] vertices, FlatVector center)
        {
            int result = -1;
            float minDistance = float.MaxValue;
            for (int i = 0; i < vertices.Length; i++)
            {
                float distance = FlatMath.Distance(vertices[i], center);
                if (distance < minDistance)
                {
                    result = i;
                    minDistance = distance;
                }
            }


            return result;
        }
        public static bool IntersectPolygonPolygon(FlatVector[] verticesA, 
                FlatVector[] verticesB, FlatVector centerA, FlatVector centerB,
                out FlatVector normal, out float depth)
        {
            normal = FlatVector.Zero;
            depth = float.MaxValue;



            for (int i = 0; i < verticesA.Length; i++)
            {
                FlatVector va = verticesA[i];
                FlatVector vb = verticesA[(i + 1) % verticesA.Length];

                FlatVector eage = vb - va;
                FlatVector axis = FlatMath.Normalize(new FlatVector(-eage.y, eage.x));

                ProjectVertices(verticesA, axis, out float minA, out float maxA);
                ProjectVertices(verticesB, axis, out float minB, out float maxB);
                if (maxA < minB || maxB < minA)  
                {
                    return false;
                }


                float axisDepth = Mathf.Min(maxA - minB, maxB - minA);
                if (axisDepth < depth)
                {
                    normal = axis;
                    depth = axisDepth;

                }
            }
            for (int i = 0; i < verticesB.Length; i++)
            {
                FlatVector va = verticesB[i];
                FlatVector vb = verticesB[(i + 1) % verticesB.Length];

                FlatVector eage = vb - va;
                FlatVector axis = FlatMath.Normalize(new FlatVector(-eage.y, eage.x));

                ProjectVertices(verticesA, axis, out float minA, out float maxA);
                ProjectVertices(verticesB, axis, out float minB, out float maxB);
                if (maxA < minB || maxB < minA)
                {
                    return false;
                }


                float axisDepth = Mathf.Min(maxA - minB, maxB - minA);
                if (axisDepth < depth)
                {
                    normal = axis;
                    depth = axisDepth;

                }
            }

            FlatVector direction = centerB - centerA;

            if (FlatMath.Dot(direction, normal) < 0f)
            {

                normal = new FlatVector(-normal.x, -normal.y);
            }

            return true;
        }
        private static void ProjectCirlce(FlatVector center, float radius, FlatVector axis, out float min, out float max)
        {
            FlatVector p1 = center - axis * radius;
            FlatVector p2 = center + axis * radius;
            float dot1 = FlatMath.Dot(p1, axis);
            float dot2 = FlatMath.Dot(p2, axis);
            if (dot1 > dot2)
            {
                min = dot2;
                max = dot1;
            }
            else
            {
                min = dot1;
                max = dot2;
            }
        }
        private static void ProjectVertices(FlatVector[] vertices, FlatVector axis, out float min, out float max)
        {
            min = float.MaxValue;
            max = float.MinValue;

            for (int i = 0; i < vertices.Length; i++)
            {
                FlatVector v = vertices[i];
                float dot = FlatMath.Dot(v, axis);
                if (dot < min)
                {
                    min = dot;
                }
                if (dot > max)
                {
                    max = dot;
                }
            }
        }
        public static bool IntersectCircleCircle(FlatVector centerA, float radiusA, FlatVector centerB, float radiusB,
            out FlatVector normal, out float depth)
        {
            float distance = FlatMath.Distance(centerA, centerB);
            float radiiSum = radiusA + radiusB;

            if (distance > radiiSum)
            {
                normal = FlatVector.Zero;
                depth = 0;
                return false;
            }

            if (distance < Mathf.Abs(radiusA - radiusB))
            {
                normal = FlatVector.Zero;
                depth = 0;
                return false;
            }

            normal = FlatMath.Normalize(centerB - centerA);//A->B
            depth = radiiSum - distance;
            return true;
        }
    }
}
