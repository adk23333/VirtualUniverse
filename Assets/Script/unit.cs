
public class GlobalVar
{
    private static GlobalVar instance;
    public float gravityConstant = 1.0f; // 引力常数，可以根据需要调整
    public int numberOfStars = 1000; // 实例数量
    public float density = 1.0f; //密度
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


