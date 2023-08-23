using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Star
{
    public Vector2 position;
    public float size;
    public float mass;
    public float density;

    public Star(Vector2 position, float size, float mass, float density)
    {
        this.position = position;
        this.size = size;
        this.mass = mass;
        this.density = density;
    }
}

public class InitStar : MonoBehaviour
{
    public GameObject prefab; // 预制体


    void Start()
    {
        List<Star> stars = GenerateCircles(
            GlobalVar.Instance.numberOfStars,
            GlobalVar.Instance.density,
            GlobalVar.Instance.massLower,
            GlobalVar.Instance.massUpper,
            GlobalVar.Instance.coordinate);
        int i = 1;
        foreach (Star star in stars)
        {
            // 生成预制体实例
            GameObject instance = Instantiate(prefab, star.position, Quaternion.identity);

            // 给每个实例命名
            instance.name = prefab.tag + i;
            i++;

            // 设置大小
            float scale = star.size;
            instance.transform.localScale = new Vector2(scale, scale);
            Rigidbody2D rb = instance.GetComponent<Rigidbody2D>(); // 获取游戏对象的刚体组件
            if (rb != null) { rb.mass = star.mass; }
        }
    }

    public List<Star> GenerateCircles(int n, float density, float massLower, float massUpper, float coordinate)
    {
        List<Star> star = new List<Star>();

        for (int i = 0; i < n; i++)
        {
            Star newStar;
            int j = 0;
            do
            {
                if (j > 100) { throw new TimeoutException("GenerateCircles Timeout"); }
                j++;
                float mass = UnityEngine.Random.Range(massLower, massUpper);
                newStar = new Star(
                    new Vector2(
                        UnityEngine.Random.Range(-coordinate, coordinate),
                        UnityEngine.Random.Range(-coordinate, coordinate)
                        ),
                    Mathf.Sqrt(mass / density / Mathf.PI),
                    mass,
                    density
                    );
                
            } while (IsOverlapping(newStar, star));

            star.Add(newStar);
        }

        return star;
    }

    private bool IsOverlapping(Star newStar, List<Star> existingStars)
    {
        foreach (var star in existingStars)
        {
            if (Vector2.Distance(newStar.position, star.position) < (newStar.size + star.size))
            {
                return true;
            }
        }

        return false;
    }


}
