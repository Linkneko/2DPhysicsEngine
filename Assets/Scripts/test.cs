using FlatPhysics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    FlatWorld world;
    int bodyCount = 10;

    void Start()
    {
        world = new FlatWorld();
        float width = Screen.width;
        float height = Screen.height;
        float depth = width * 0.02f;
        float Zdepth = 5f;

        for (int i = 0; i < bodyCount; i++)
        {
            FlatBody body = null;
            int type = Random.Range(0, 2);
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

            world.AddBody(body);


        }
    }
    void Draw()
    {
        for(int i = 0; i < world.BodyCount; i++)
        {
            FlatBody body = null;
            if(world.GetBody(i, out body))
            {
                if (body.ShapeType == ShapeType.Circle)
                {
                    FlatDraw.DrawCircle(body);
                }
                else if (body.ShapeType == ShapeType.Box)
                {
                    FlatDraw.DrawBox(body);
                }
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        Draw();

        
        float dx = 0f, dy = 0f;
        float speed = 10f;
        if (Input.GetKey(KeyCode.A)){dx = -1;}
        if (Input.GetKey(KeyCode.D)){dx = 1;}
        if (Input.GetKey(KeyCode.W)){dy = 1;}
        if (Input.GetKey(KeyCode.S)){dy = -1;}
        FlatBody body = null;
        if (world.GetBody(0, out body))
        {
            body.Move(new FlatVector(dx * speed * Time.deltaTime, dy * speed * Time.deltaTime));

        if (Input.GetKey(KeyCode.R))
                body.Rotate(Mathf.PI * .5f * Time.deltaTime);

        }
        world.Step(Time.deltaTime);
    }
}
