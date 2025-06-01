using UnityEngine;

public class ResourcesHandler : MonoBehaviour
{
    public static ResourcesHandler main;

    public static int money;
    public static int coal;
    public static int iron;
    public static int oil;

    public static int co2LvlIncreaser, co2LvlDecreaser, co2Lvl;

    private void Awake()
    {
        main = this;
    }

    public void GetMoney()
    {
        money++;    
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            money++;
        }
    }

}
