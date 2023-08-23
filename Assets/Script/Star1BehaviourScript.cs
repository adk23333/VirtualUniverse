using UnityEngine;
using UnityEngine.InputSystem;

public class Star1Controller: MonoBehaviour
{
    private float gravityConstant = GlobalVar.Instance.gravityConstant;
    public string baseStarTag = "BaseStar"; // BaseStarԤ����ı�ǩ

    void Update()
    {
        // ��ȡ����BaseStarԤ����
        GameObject[] baseStars = GameObject.FindGameObjectsWithTag(baseStarTag);
        float gameMass = gameObject.GetComponent<Rigidbody2D>().mass;

        Vector2 totalForce = Vector2.zero;

        // ��������BaseStarԤ���壬��������
        foreach (GameObject baseStar in baseStars)
        {
            if (baseStar != gameObject) // �ų�����
            {
                Vector2 direction = baseStar.transform.position - transform.position;
                float distance = direction.magnitude;
                float baseMass = baseStar.GetComponent<Rigidbody2D>().mass;
                Vector2 force = gravityConstant * gameMass * baseMass * direction.normalized / (distance * distance);
                totalForce += force;
            }
        }

        // Ӧ������
        GetComponent<Rigidbody2D>().AddForce(totalForce);
    }

}
