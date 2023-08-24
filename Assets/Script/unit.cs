
using UnityEngine;

public class GlobalVar
{
    private static GlobalVar instance;
    public float gravityConstant = 10.0f; // �������������Ը�����Ҫ����
    public int numberOfStars = 200; // ʵ������
    public float velocity = 40.0f; //�ٶ�
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


