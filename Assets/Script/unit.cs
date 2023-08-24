
using UnityEngine;

public class GlobalVar
{
    private static GlobalVar instance;
    public float gravityConstant = 10.0f; // 引力常数，可以根据需要调整
    public int numberOfStars = 200; // 实例数量
    public float velocity = 40.0f; //速度
    public float massLower = 5.0f;
    public float massUpper = 50.0f;
    public float coordinate = 800f;
    public int powerOfDistance = 1;


    private GlobalVar() { }

    public static GlobalVar Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GlobalVar();
            }
            return instance;
        }
    }
}


