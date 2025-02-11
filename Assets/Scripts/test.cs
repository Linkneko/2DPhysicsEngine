using FlatPhysics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    FlatWorld world;
    int bodyCount = 10;
    float width = Screen.width;
    float height = Screen.height;
    
    float Zdepth = 5f;
    void Start()
    {
        world = new FlatWorld();
        float depth = width * 0.02f;

        for (int i = 0; i < bodyCount; i++)
        {
            FlatBody body = null;
            int type = Random.Range(0, 2);
            float x = Random.Range(depth, width - depth);
            float y = Random.Range(depth, height - depth);
            bool isStatic = Random.Range(0, 4) == 0;
            if(i == 0) isStatic = false;
            // 将坐标转换到世界坐标
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(x, y, Zdepth));
            if (type == (int)ShapeType.Circle)
            {
                if (!FlatBody.CreateCircleBody(.5f, new FlatVector(worldPosition.x, worldPosition.y), 2f, isStatic, 0.5f, out body, out string error))
                {
                    throw new System.Exception(error);
                }
            }
            else if (type == (int)ShapeType.Box)
            {
                if (!FlatBody.CreateBoxBody(.885f, .885f, new FlatVector(worldPosition.x, worldPosition.y), 2f, isStatic, 0.5f, out body, out string error))
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
            body.AddForce(new FlatVector(dx * speed, dy * speed));

        if (Input.GetKey(KeyCode.R))
                body.Rotate(Mathf.PI * .5f * Time.deltaTime);

        }

        Vector3 worldpos = Camera.main.ScreenToWorldPoint(new Vector3(width, height, Zdepth));

        for (int i = 0; i < world.BodyCount; i++)
        {
            if (!world.GetBody(i, out body))
            {
                
            }
            if(body.Position.x < -worldpos.x){body.MoveTo(new FlatVector(worldpos.x, body.Position.y));}
            if(body.Position.x > worldpos.x){body.MoveTo(new FlatVector(-worldpos.x, body.Position.y));}
            if(body.Position.y < -worldpos.y){body.MoveTo(new FlatVector(body.Position.x, worldpos.y));}
            if(body.Position.y > worldpos.y){body.MoveTo(new FlatVector(body.Position.x, -worldpos.y));}
        }

        world.Step(Time.deltaTime);
    }
}
