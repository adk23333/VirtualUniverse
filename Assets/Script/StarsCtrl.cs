using System;
using System.Collections.Generic;
using UnityEngine;

public struct Star
{
    public Vector2 position;
    public Vector2 velocity;
    public float size;
    public float mass;
    public float density;

    public Star(Vector2 position, Vector2 velocity, float size, float mass, float density)
    {
        this.position = position;
        this.velocity = velocity;
        this.size = size;
        this.mass = mass;
        this.density = density;
    }
}

public class StarsCtrl : MonoBehaviour
{
    public GameObject prefab; // 预制体
    public string baseStarTag = "BaseStar"; // BaseStar预制体的标签

    public List<GameObject> stars = new List<GameObject>();
    public RecyclingListView starInfoList;


    void Start()
    {
        InitStar();
        starInfoList.ItemCallback = PopulateListItem;
    }

    private void Update()
    {
        MoveCamOfLarestStar();
    }

    public void DelStarInfoListItem(GameObject go)
    {
        stars.Remove(go);
        starInfoList.RowCount = stars.Count;
    }

    public void SortStarInfos(GameObject go)
    {
        stars.Remove(go);
        stars.Insert(GetIndex(go.GetComponent<Rigidbody2D>().mass), go);
    }

    private void PopulateListItem(RecyclingListViewItem item, int rowIndex)
    {
        var child = item as StarInfoItem;
        child.starName = stars[rowIndex].transform.name;
        child.mass = stars[rowIndex].GetComponent<Rigidbody2D>().mass.ToString();
    }

    private void MoveCamOfLarestStar()
    {
        GameObject[] stars = GameObject.FindGameObjectsWithTag(baseStarTag);
        float maxMass = 0f;
        GameObject maxMassStar = null;
        foreach (GameObject star in stars)
        {
            Rigidbody2D rb = star.GetComponent<Rigidbody2D>();
            if (rb.mass > maxMass)
            {
                maxMass = rb.mass;
                maxMassStar = star;
            }
        }

        Camera.main.gameObject.transform.position = new Vector3(maxMassStar.transform.position.x, maxMassStar.transform.position.y, -10);
    }

    private void InitStar()
    {
        List<Star> stars = GenerateCircles(
            GlobalVar.Instance.numberOfStars,
            BaseStarCtrl.density,
            GlobalVar.Instance.velocity,
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
            if (rb != null)
            {
                rb.mass = star.mass;
                rb.velocity = star.velocity;
            }
            if (this.stars.Count != 0)
            {
                this.stars.Insert(GetIndex(rb.mass), instance);

            }
            else
            {
                this.stars.Add(instance);
            }
            
        }
    }

    private int GetIndex(float mass)
    {
        for(int i=0; i<stars.Count; i++)
        {
            if (stars[i].GetComponent<Rigidbody2D>().mass < mass)
            {
                return i;
            }
        }
        return stars.Count;

    }

    private List<Star> GenerateCircles(int n, float density, float velocity, float massLower, float massUpper, float coordinate)
    {
        List<Star> stars = new List<Star>();

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
                        UnityEngine.Random.Range(-2 * coordinate, 2 * coordinate),
                        UnityEngine.Random.Range(-coordinate, coordinate)
                        ),
                    new Vector2(
                        UnityEngine.Random.Range(-velocity, velocity),
                        UnityEngine.Random.Range(-velocity, velocity)
                        ),
                    Mathf.Sqrt(mass / density / Mathf.PI),
                    mass,
                    density
                    );

            } while (IsOverlapping(newStar, stars));

            stars.Add(newStar);
        }

        return stars;
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
