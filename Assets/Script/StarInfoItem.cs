using TMPro;
using UnityEngine;

public class StarInfoItem : RecyclingListViewItem
{
    public string starName;
    public string mass;

    private void Update()
    {
        Transform nameTf = transform.Find("Name");
        Transform massTf = transform.Find("Mass");

        TextMeshProUGUI nameText = nameTf.GetComponent<TextMeshProUGUI>();
        nameText.text = starName;

        TextMeshProUGUI massText = massTf.GetComponent<TextMeshProUGUI>();
        massText.text = mass;
    }

    public void SetStarInfo(string starName, string mass)
    {
        this.starName = starName;
        this.mass = mass;
    }

   
}


