using Unity.VisualScripting;
using UnityEngine;

public enum Resource
{
    Carv�o,
    Ferro,
    BarraDeFerro,
    Petr�leo,
    Energia,
    
}

public enum OverclockType
{
    Energy,
    Resource
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

    public static int co2Lvl;

    public static int totalROverclocks, totalEOverclocks;
    public static int currentROverclocks, currentEOverclocks;

    private void Awake()
    {
        main = this;
        money = 0;
        coal = 0;
        iron = 0;
        oil = 0;
        energy = 0;
        ironBar = 0;
        co2Lvl = 0;
        totalROverclocks = 0;
        totalEOverclocks = 0;
        currentEOverclocks = 0;
        currentROverclocks = 0;
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

    public OverclockType GetOverclockType(string type)
    {
        switch (type)
        {
            case "Resource": return OverclockType.Resource;
            case "Energy": return OverclockType.Energy;
        }

        return default;
    }
}
