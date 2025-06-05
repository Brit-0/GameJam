using UnityEngine;

public enum Resource
{
    Carv�o,
    Ferro,
    BarraDeFerro,
    Petr�leo,
    Energia,
    
}

public class ResourcesHandler : MonoBehaviour
{
    public static ResourcesHandler main;

    public static int money;
    public static int coal;
    public static int iron;
    public static int oil;
    public static int energy;
    public static int ironBar;

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
        if (Input.GetKey(KeyCode.Q) && Input.GetKeyDown(KeyCode.E))
        {
            money += 100;
        }
    }

    public int GetResource(Resource resource)
    {
        switch (resource)
        {
            case Resource.Carv�o: return coal;
            case Resource.Ferro: return iron;
            case Resource.Petr�leo: return oil;
            case Resource.Energia: return energy;
            case Resource.BarraDeFerro: return ironBar;
        }

        return 0;
    }

    public void UpdateResource(Resource resource, int amount)
    {
        switch (resource)
        {
            case Resource.Carv�o: coal = amount; break;
            case Resource.Ferro: iron = amount; break;
            case Resource.Petr�leo: oil = amount; break;
            case Resource.Energia: energy = amount; break;
            case Resource.BarraDeFerro: ironBar = amount; break;
        }
    }

}
