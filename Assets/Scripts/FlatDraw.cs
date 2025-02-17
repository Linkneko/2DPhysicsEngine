using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.HableCurve;

namespace FlatPhysics
{
    public static class FlatDraw
    {
        // �����ֵ䣺FlatBody ��Ӧ�� LineRenderer
        private static Dictionary<FlatBody, LineRenderer> bodyLineMap = new Dictionary<FlatBody, LineRenderer>();
        private const int Circlesegments = 36;
        public static void DrawBox(FlatBody body)
        {
            // ����Ѵ��ڣ�ֱ�Ӹ���λ��
            if (bodyLineMap.TryGetValue(body, out LineRenderer lr))
            {
                UpdateBoxPosition(lr, body);
                return;
            }
            // �½� LineRenderer ������
            GameObject box = new GameObject("Box");
            lr = box.AddComponent<LineRenderer>();
            bodyLineMap[body] = lr;

            float lineWidth = 0.03f;


            lr.startWidth = lineWidth;
            lr.endWidth = lineWidth;
            lr.loop = true;
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.startColor = body.color;
            lr.endColor = body.color;
            UpdateBoxPosition(lr, body);
        }
        public static void DrawCircle(FlatBody body)
        {
            // ����Ѵ��ڣ�ֱ�Ӹ���λ��
            if (bodyLineMap.TryGetValue(body, out LineRenderer lr))
            {
                UpdateCirclePosition(lr, body);
                return;
            }

            // �½� LineRenderer ������
            GameObject circle = new GameObject("Circle");
            lr = circle.AddComponent<LineRenderer>();
            bodyLineMap[body] = lr;


            float lineWidth = 0.03f;
            

            lr.startWidth = lineWidth;
            lr.endWidth = lineWidth;
            lr.loop = true;
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.startColor = body.color;
            lr.endColor = body.color;

            // ��һ�δ���ʱ����λ��
            UpdateCirclePosition(lr, body);
        }
        private static void UpdateBoxPosition(LineRenderer lr, FlatBody body)
        {
            float zDistance = 5f;
            FlatVector[] vertices = body.GetTransformedVertices();
            lr.positionCount = 4;
            lr.SetPosition(0, new Vector3(vertices[0].x, vertices[0].y, zDistance));
            lr.SetPosition(1, new Vector3(vertices[1].x, vertices[1].y, zDistance));
            lr.SetPosition(2, new Vector3(vertices[2].x, vertices[2].y, zDistance));
            lr.SetPosition(3, new Vector3(vertices[3].x, vertices[3].y, zDistance));
            lr.loop = true;
        }
        private static void UpdateCirclePosition(LineRenderer lr, FlatBody body)
        {
            
            float zDistance = 5f;


            // ����Բ��λ�ú���״
            lr.positionCount = Circlesegments;
            float angleStep = 360f / Circlesegments;

            for (int i = 0; i < Circlesegments; i++)
            {
                float angle = i * angleStep * Mathf.Deg2Rad;
                float x = body.Position.x + body.Radius * Mathf.Cos(angle);
                float y = body.Position.y + body.Radius * Mathf.Sin(angle);
                lr.SetPosition(i, new Vector3(x, y, zDistance));
            }
        }











        // ��ѡ������������ʱ������
        public static void RemoveBody(FlatBody body)
        {
            if (bodyLineMap.TryGetValue(body, out LineRenderer lr))
            {
                GameObject.Destroy(lr.gameObject);
                bodyLineMap.Remove(body);
            }
        }
    }
}
