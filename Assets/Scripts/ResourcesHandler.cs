using UnityEngine;

public class ResourcesHandler : MonoBehaviour
{
    public static ResourcesHandler main;

    [SerializeField] private HUDHandler hud;

    [SerializeField] private int money;
    [SerializeField] private int coal;

    private void Awake()
    {
        main = this;
    }

    public int Coal
    {
        get
        {
            return coal;
        }

        set
        {
            coal = value;
            hud.updateResources();
        }
    }
    public int Money
    {
        get
        {
            return money;
        } 
        
        set
        {
            money = value;
            hud.updateResources();
        }
    }

    public void GetMoney()
    {
        Money++;    
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Money++;
        }
    }
}
