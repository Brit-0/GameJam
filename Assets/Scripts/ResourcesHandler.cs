using UnityEngine;

public class ResourcesHandler : MonoBehaviour
{
    public static ResourcesHandler main;

    public static int money;
    public static int coal;
    public static int iron;
    public static int oil;
    public static int energy;

    public static int co2LvlIncreaser, co2LvlDecreaser, co2Lvl;

    private void Awake()
    {
        main = this;
    }

    public void GetMoney()
    {
        money += 10;    
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            GetMoney();
        }
    }

    public int GetResource(Resource resource)
    {
        switch (resource)
        {
            case Resource.Carvão: return coal;
            case Resource.Ferro: return iron;
            case Resource.Petróleo: return oil;
        }

        return 0;
    }

    public void UpdateResource(Resource resource, int amount)
    {
        switch (resource)
        {
            case Resource.Carvão: coal = amount; break;
            case Resource.Ferro: iron = amount; break;
            case Resource.Petróleo: oil = amount; break;
        }
    }

}
