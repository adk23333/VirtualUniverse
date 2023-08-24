
public class GlobalVar
{
    private static GlobalVar instance;
    public float gravityConstant = 10.0f; // �������������Ը�����Ҫ����
    public int numberOfStars = 1000; // ʵ������
    public float velocity = 40.0f; //�ٶ�
    public float massLower = 50.0f;
    public float massUpper = 500.0f;
    public float coordinate = 800f;
    

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


