using System.Linq;
using UnityEngine;

public class BaseStarCtrl : MonoBehaviour
{
    public string baseStarTag = "BaseStar"; // BaseStar预制体的标签
    public static float density = 1.0f; //密度
    

    private float gravityConstant = GlobalVar.Instance.gravityConstant;
    

    void Update()
    {
        // 获取所有BaseStar预制体
        GameObject[] baseStars = GameObject.FindGameObjectsWithTag(baseStarTag);
        float gameMass = gameObject.GetComponent<Rigidbody2D>().mass;

        Vector2 totalForce = Vector2.zero;

        // 遍历所有BaseStar，计算引力
        foreach (GameObject baseStar in baseStars)
        {
            if (baseStar != gameObject) // 排除自身
            {
                Vector2 direction = baseStar.transform.position - transform.position;
                float distance = direction.magnitude;
                float baseMass = baseStar.GetComponent<Rigidbody2D>().mass;
                Vector2 force = gravityConstant * gameMass * baseMass * direction.normalized / Mathf.Pow(distance, GlobalVar.Instance.powerOfDistance);
                totalForce += force;
            }
        }

        // 应用引力
        GetComponent<Rigidbody2D>().AddForce(totalForce);
        
    }

    private void OnDestroy()
    {
        GameObject starCtrlObj = GameObject.FindGameObjectsWithTag("GameController").First();
        var starCtrl = starCtrlObj.GetComponent<StarsCtrl>();
        starCtrl.DelStarInfoListItem(gameObject);
    }

    

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Collider2D otherCollider = collision.collider;
        float gameMass = gameObject.GetComponent<Rigidbody2D>().mass;
        float otherMass = otherCollider.GetComponent<Rigidbody2D>().mass;
        if (gameMass >= otherMass)
        {
            MergeStar(gameObject, otherCollider.gameObject);
        }
        else
        {
            MergeStar(otherCollider.gameObject, gameObject);
        }
    }

    private void MergeStar(GameObject mainGO, GameObject otherGo)
    {
        Rigidbody2D mainRB = mainGO.GetComponent<Rigidbody2D>();
        Rigidbody2D otherRB = otherGo.GetComponent<Rigidbody2D>();
        float mass = mainRB.mass + otherRB.mass;
        mainGO.transform.position = (mainGO.transform.position * mainRB.mass + otherGo.transform.position * otherRB.mass) / mass;

        mainRB.velocity = (mainRB.velocity * mainRB.mass + otherRB.velocity * otherRB.mass) / mass;
        mainRB.mass = mass;
        float size = Mathf.Sqrt(mass / density / Mathf.PI);
        mainGO.transform.localScale = new Vector2(size, size);

        Destroy(otherGo);

        GameObject starCtrlObj = GameObject.FindGameObjectsWithTag("GameController").First();
        var starCtrl = starCtrlObj.GetComponent<StarsCtrl>();
        starCtrl.SortStarInfos(mainGO);
    }

}
