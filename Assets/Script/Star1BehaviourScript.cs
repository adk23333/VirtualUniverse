using UnityEngine;
using UnityEngine.InputSystem;

public class Star1Controller: MonoBehaviour
{
    private float gravityConstant = GlobalVar.Instance.gravityConstant;
    public string baseStarTag = "BaseStar"; // BaseStar预制体的标签

    void Update()
    {
        // 获取所有BaseStar预制体
        GameObject[] baseStars = GameObject.FindGameObjectsWithTag(baseStarTag);
        float gameMass = gameObject.GetComponent<Rigidbody2D>().mass;

        Vector2 totalForce = Vector2.zero;

        // 遍历所有BaseStar预制体，计算引力
        foreach (GameObject baseStar in baseStars)
        {
            if (baseStar != gameObject) // 排除自身
            {
                Vector2 direction = baseStar.transform.position - transform.position;
                float distance = direction.magnitude;
                float baseMass = baseStar.GetComponent<Rigidbody2D>().mass;
                Vector2 force = gravityConstant * gameMass * baseMass * direction.normalized / (distance * distance);
                totalForce += force;
            }
        }

        // 应用引力
        GetComponent<Rigidbody2D>().AddForce(totalForce);
    }

}
