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
    public GameObject prefab; // Ԥ����


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
            // ����Ԥ����ʵ��
            GameObject instance = Instantiate(prefab, star.position, Quaternion.identity);

            // ��ÿ��ʵ������
            instance.name = prefab.tag + i;
            i++;

            // ���ô�С
            float scale = star.size;
            instance.transform.localScale = new Vector2(scale, scale);
            Rigidbody2D rb = instance.GetComponent<Rigidbody2D>(); // ��ȡ��Ϸ����ĸ������
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
