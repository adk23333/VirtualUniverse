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
    public GameObject prefab; // Ԥ����


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
            // ����Ԥ����ʵ��
            GameObject instance = Instantiate(prefab, circle.position, Quaternion.identity);

            // ��ÿ��ʵ������
            instance.name = prefab.tag + i;
            i++;

            // ���ô�С
            float scale = circle.size;
            instance.transform.localScale = new Vector2(scale, scale);
            Rigidbody2D rb = instance.GetComponent<Rigidbody2D>(); // ��ȡ��Ϸ����ĸ������
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
