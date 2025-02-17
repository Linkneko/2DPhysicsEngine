using FlatPhysics;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using UnityEngine;
using UnityEngine.UI;

public class test : MonoBehaviour
{
    FlatWorld world;
    float width = Screen.width;
    float height = Screen.height;
    public Text myText;
    float Zdepth = 5f;
    private float timer = 0f;
    Stopwatch sw = new Stopwatch();
    List<FlatVector> contactList;
    void Start()
    {
        world = new FlatWorld();
        float depth = width * 0.02f;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(width, height, Zdepth));
        if (!FlatBody.CreateBoxBody(worldPosition.x * 2 - 1f, .5f, new FlatVector(0, -worldPosition.y*0.8f), 0, true, 0.5f, out FlatBody ground, out string error))
        {
            throw new System.Exception(error);
        }
        ground.color = Color.white;
        world.AddBody(ground);

        contactList = world?.contactPointsList;
        StartCoroutine(UpdateStepTime());
#if false
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
#endif
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
        float depth = width * 0.02f;
        FlatBody body = null;
        
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 MousePosition = Input.mousePosition;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(MousePosition.x, MousePosition.y, Zdepth));

            
            if (!FlatBody.CreateCircleBody(.5f, new FlatVector(worldPosition.x, worldPosition.y), 2f, false, 0.5f, out body, out string error))
            {
                throw new System.Exception(error);
            }
            world.AddBody(body);
        }
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 MousePosition = Input.mousePosition;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(MousePosition.x, MousePosition.y, Zdepth));

            if (!FlatBody.CreateBoxBody(.885f, .885f, new FlatVector(worldPosition.x, worldPosition.y), 2f, false, 0.5f, out body, out string error))
            {
                throw new System.Exception(error);
            }
            world.AddBody(body);
        }
        Draw();


        Vector3 worldpos = Camera.main.ScreenToWorldPoint(new Vector3(width+5f*depth, height + 5f*depth, Zdepth));

        for (int i = 0; i < world.BodyCount; i++)
        {
            if (!world.GetBody(i, out body))
            {
            }
            FlatAABB aabb = body.GetAABB();
            if (aabb.Min.x < -worldpos.x || aabb.Max.x > worldpos.x || aabb.Min.y < -worldpos.y || aabb.Max.y > worldpos.y)
            {
                world.RemoveBody(body);
                FlatDraw.RemoveBody(body);
            }
            
        }

        sw.Start();
        world.Step(Time.deltaTime, 10);
        sw.Stop();
        if (Input.GetKey(KeyCode.Space))
        {
            UnityEngine.Debug.Log("Step time: " + sw.Elapsed.TotalMilliseconds + "ms");
        }
    }
    IEnumerator UpdateStepTime()
    {
        while (true)
        {
            myText.text = "Body Count: " + world.BodyCount;
            myText.text += "\nStep time: " + sw.Elapsed.TotalMilliseconds.ToString("F2") + "ms";
            sw.Reset();
            yield return new WaitForSeconds(.2f);
        }
    }
}
