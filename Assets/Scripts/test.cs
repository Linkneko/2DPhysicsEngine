using FlatPhysics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    List<FlatBody> bodyList;
    void Start()
    {
        float width = Screen.width;
        float height = Screen.height;
        float depth = width * 0.02f;
        float Zdepth = 5f;
        int bodyCount = 10;
        bodyList = new List<FlatBody>(10);
        for (int i = 0; i < bodyCount; i++)
        {
            FlatBody body = null;
            int type = Random.Range(1, 2);
            float x = Random.Range(depth, width - depth);
            float y = Random.Range(depth, height - depth);
            // 将坐标转换到屏幕空间
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(x, y, Zdepth));
            if (type == (int)ShapeType.Circle)
            {
                if (!FlatBody.CreateCircleBody(.5f, new FlatVector(worldPosition.x, worldPosition.y), 2f, false, 0.5f, out body, out string error))
                {
                    throw new System.Exception(error);
                }
            }
            else if (type == (int)ShapeType.Box)
            {
                if (!FlatBody.CreateBoxBody(1f, 1f, new FlatVector(worldPosition.x, worldPosition.y), 2f, false, 0.5f, out body, out string error))
                {
                    throw new System.Exception(error);
                }
            }
            else
            {
                throw new System.Exception("Invalid shape type");
            }

            bodyList.Add(body);


        }
    }
    void Draw()
    {
        for(int i = 0; i < bodyList.Count; i++)
        {
            FlatBody body = bodyList[i];
            if(body.ShapeType == ShapeType.Circle)
            {
                FlatDraw.DrawCircle(body);
            }
            else if(body.ShapeType == ShapeType.Box)
            {
                FlatDraw.DrawBox(body);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        Draw();
        for (int i = 0; i < bodyList.Count; i++)
        {
            bodyList[i].isColliding = false;
        }

        for (int i = 0; i < bodyList.Count; i++)
        {
            FlatBody bodyA = bodyList[i];
            for(int j = i + 1; j < bodyList.Count; j++)
            {
                FlatBody bodyB = bodyList[j];
                if(Collisions.IntersectPolygonPolygon(bodyA.GetTransformedVertices(),
                    bodyB.GetTransformedVertices(), out FlatVector normal, out float depth))
                {
                        bodyA.isColliding = true;
                        bodyB.isColliding = true;
                    bodyA.Move(-normal * depth * 0.5f);
                    bodyB.Move(normal * depth * 0.5f);
                }
            }
        }
        
        float dx = 0f, dy = 0f;
        float speed = 10f;
        if (Input.GetKey(KeyCode.A)){dx = -1;}
        if (Input.GetKey(KeyCode.D)){dx = 1;}
        if (Input.GetKey(KeyCode.W)){dy = 1;}
        if (Input.GetKey(KeyCode.S)){dy = -1;}

        bodyList[0].Move(new FlatVector(dx * speed * Time.deltaTime, dy * speed * Time.deltaTime));

        if (Input.GetKey(KeyCode.R))
            bodyList[0].Rotate(Mathf.PI * .5f * Time.deltaTime);
    }
}
