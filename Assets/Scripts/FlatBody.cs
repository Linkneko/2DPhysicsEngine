
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FlatPhysics
{
    public enum ShapeType
    {
        Circle = 0,
        Box = 1
    }

    public sealed class FlatBody
    {
        private FlatVector position;
        private FlatVector linearVelocity;
        private float rotation;
        private float rotationalVelocity;
        public Color color;
        private FlatVector force;

        public readonly float Density;
        public readonly float Mass;
        public readonly float InvMass;
        public readonly float Restitution;
        public readonly float Area;
        public bool isColliding;
        public readonly bool IsStatic;

        public readonly float Radius;
        public readonly float Width;
        public readonly float Height;

        private readonly FlatVector[] vertices;

        private FlatVector[] transformedVertices;
        private FlatAABB aabb;
        private bool transformUpdateRequired;
        private bool aabbUpdteRequired;

        public readonly ShapeType ShapeType;

        public FlatVector Position
        {
            get { return this.position; }
        }

        public FlatVector LinearVelocity
        {
            get { return this.linearVelocity; }
            internal set { this.linearVelocity = value; }
        }

        private FlatBody(FlatVector position, float density, float mass, float restitution, float area,
            bool isStatic, float radius, float width, float height, ShapeType shapeType)
        {
            this.position = position;
            this.linearVelocity = FlatVector.Zero;
            this.rotation = 0f;
            this.rotationalVelocity = 0f;
            color = isStatic? Color.red : new Color(
                Random.Range(0.6f, 1f),
                Random.Range(0.6f, 1f),
                Random.Range(0.6f, 1f)
            );
            this.force = FlatVector.Zero;

            this.Density = density;
            this.Mass = mass;
            this.Restitution = restitution;
            this.Area = area;

            this.IsStatic = isStatic;
            this.Radius = radius;
            this.Width = width;
            this.Height = height;
            this.ShapeType = shapeType;

            if (!this.IsStatic)
            {
                this.InvMass = 1f / this.Mass;
            }
            else
            {
                this.InvMass = 0f;
            }

            if (this.ShapeType is ShapeType.Box)
            {
                this.vertices = FlatBody.CreateBoxVertices(this.Width, this.Height);
                this.transformedVertices = new FlatVector[this.vertices.Length];
            }
            else
            {
                this.vertices = null;
                this.transformedVertices = null;
            }

            this.transformUpdateRequired = true;
            this.aabbUpdteRequired = false;
        }

        private static FlatVector[] CreateBoxVertices(float width, float height)
        {
            float left = -width / 2f;
            float right = left + width;
            float bottom = -height / 2f;
            float top = bottom + height;

            FlatVector[] vertices = new FlatVector[4];
            vertices[0] = new FlatVector(left, top);
            vertices[1] = new FlatVector(right, top);
            vertices[2] = new FlatVector(right, bottom);
            vertices[3] = new FlatVector(left, bottom);

            return vertices;
        }


        public FlatVector[] GetTransformedVertices()
        {
            if (this.transformUpdateRequired)
            {
                FlatTransform transform = new FlatTransform(this.position, this.rotation);

                for (int i = 0; i < this.vertices.Length; i++)
                {
                    FlatVector v = this.vertices[i];
                    this.transformedVertices[i] = FlatVector.Transform(v, transform);
                }
            }

            this.transformUpdateRequired = false;
            return this.transformedVertices;
        }


        internal void Step(float time, FlatVector gravity, int iterations)
        {
            if(this.IsStatic)
                return;
            time /= iterations;
            this.linearVelocity += force * InvMass * time;
            linearVelocity += gravity * time;

            this.position += this.linearVelocity * time;

            this.rotation += this.rotationalVelocity * time;

            this.force = FlatVector.Zero;
            this.transformUpdateRequired = true;
            this.aabbUpdteRequired = true;

        }

        public FlatAABB GetAABB()
        {
            if(this.aabbUpdteRequired)
            {
                float minX = float.MaxValue;
                float minY = float.MaxValue;
                float maxX = float.MinValue;
                float maxY = float.MinValue;
                if (this.ShapeType is ShapeType.Box)
                {
                    FlatVector[] vertices = this.GetTransformedVertices();
                    for (int i = 0; i < vertices.Length; i++)
                    {
                        FlatVector v = vertices[i];
                        if (v.x < minX) minX = v.x;
                        if (v.x > maxX) maxX = v.x;
                        if (v.y < minY) minY = v.y;
                        if (v.y > maxY) maxY = v.y;
                    }

                }else if(this.ShapeType is ShapeType.Circle)
                {
                    minX = position.x - Radius;
                    minY = position.y - Radius;
                    maxX = position.x + Radius;
                    maxY = position.y + Radius;
                }
            
                else
                {
                    throw new System.Exception("Invalid ShapeType");
                }
                this.aabb = new FlatAABB(minX, minY, maxX, maxY);
            }
            this.aabbUpdteRequired = false;
            return this.aabb;
        }
        public void Move(FlatVector amount)
        {
            this.position += amount;
            this.transformUpdateRequired = true;
        }

        public void MoveTo(FlatVector position)
        {
            this.position = position;
            this.transformUpdateRequired = true;
        }

        public void Rotate(float amount)
        {
            this.rotation += amount;
            this.transformUpdateRequired = true;
        }

        public void AddForce(FlatVector amount)
        {
            this.force = amount;
        }

        public static bool CreateCircleBody(float radius, FlatVector position, float density, bool isStatic, float restitution, out FlatBody body, out string errorMessage)
        {
            body = null;
            errorMessage = string.Empty;

            float area = radius * radius * Mathf.PI;

            restitution = FlatMath.Clamp(restitution, 0f, 1f);

            // mass = area * depth * density
            float mass = area * density;

            body = new FlatBody(position, density, mass, restitution, area, isStatic, radius, 0f, 0f, ShapeType.Circle);
            return true;
        }

        public static bool CreateBoxBody(float width, float height, FlatVector position, float density, bool isStatic, float restitution, out FlatBody body, out string errorMessage)
        {
            body = null;
            errorMessage = string.Empty;

            float area = width * height;


            restitution = FlatMath.Clamp(restitution, 0f, 1f);

            // mass = area * depth * density
            float mass = area * density;

            body = new FlatBody(position, density, mass, restitution, area, isStatic, 0f, width, height, ShapeType.Box);
            return true;
        }
    }
}