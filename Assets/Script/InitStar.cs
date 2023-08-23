using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circle
{
    public Vector2 position { get; set; }
    public float size { get; set; }
    public float mass { get; set; }
}

public class InitStar : MonoBehaviour
{
    public GameObject prefab; // 预制体


    void Start()
    {
        List<Circle> circleList = GenerateCircles(
            GlobalVar.Instance.numberOfStars, 
            GlobalVar.Instance.density,
            GlobalVar.Instance.massLower,
            GlobalVar.Instance.massUpper, 
            GlobalVar.Instance.coordinate);
        int i = 1;
        foreach (Circle circle in circleList)
        {
            // 生成预制体实例
            GameObject instance = Instantiate(prefab, circle.position, Quaternion.identity);

            // 给每个实例命名
            instance.name = prefab.tag + i;
            i++;

            // 设置大小
            float scale = circle.size;
            instance.transform.localScale = new Vector2(scale, scale);
            Rigidbody2D rb = instance.GetComponent<Rigidbody2D>(); // 获取游戏对象的刚体组件
            if (rb != null) { rb.mass = circle.mass; }
        }
    }

    public List<Circle> GenerateCircles(int n, float density, float massLower, float massUpper, float coordinate)
    {
        List<Circle> circles = new List<Circle>();

        for (int i = 0; i < n; i++)
        {
            Circle newCircle;
            int j = 0;
            do
            {
                float mass = UnityEngine.Random.Range(massLower, massUpper);
                newCircle = new Circle
                {
                    position = new Vector2(UnityEngine.Random.Range(-coordinate, coordinate), UnityEngine.Random.Range(-coordinate, coordinate)),
                    mass = mass,
                    size = Mathf.Sqrt(mass / GlobalVar.Instance.density / Mathf.PI),
                };
                j++;
                if (j > 100) { throw new TimeoutException("GenerateCircles Timeout"); }
            } while (IsOverlapping(newCircle, circles));

            circles.Add(newCircle);
        }

        return circles;
    }

    private bool IsOverlapping(Circle newCircle, List<Circle> existingCircles)
    {
        foreach (var circle in existingCircles)
        {
            if (Vector2.Distance(newCircle.position, circle.position) < (newCircle.size + circle.size))
            {
                return true;
            }
        }

        return false;
    }


}
