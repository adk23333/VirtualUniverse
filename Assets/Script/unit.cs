
public class GlobalVar
{
    private static GlobalVar instance;
    public float gravityConstant = 1.0f; // �������������Ը�����Ҫ����
    public int numberOfStars = 1000; // ʵ������
    public float density = 1.0f; //�ܶ�
    public float massLower = 50.0f;
    public float massUpper = 100.0f;
    public float coordinate = 300f;
    

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


