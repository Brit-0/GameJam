using System.Collections.Generic;
using TMPro;
using UnityEngine;

public  enum Resource
{
    Carvão,
    Ferro,
    Petróleo
}

public class NegociationsHandler : MonoBehaviour
{
    private UITweener uITweener;

    private bool isTabOpen;

    [SerializeField] private TMP_Dropdown resourceDrop;
    [SerializeField] private TMP_InputField qntInput;

    private delegate int GetVariable();
    GetVariable resourceVar;

    private int resourceAmount;

    private Dictionary<Resource, int> sellPrices = new Dictionary<Resource, int>()
    {
        {Resource.Carvão, 1},
        {Resource.Ferro, 5},
        {Resource.Petróleo, 10}
    };

    private void Awake()
    {
        uITweener = GetComponent<UITweener>();
    }

    public void ToggleTabStatus()
    {
        print("Toggle");
        if (isTabOpen)
        {
            isTabOpen = false;
        }
        else
        {
            isTabOpen = true;
        }   
    }

    public void Sell(bool sellAll)
    {
        Resource resource = (Resource) resourceDrop.value;
        int qnt;

        if (sellAll)
        {
            qnt = ResourcesHandler.main.GetResource(resource);
        }
        else
        {
            if (int.TryParse(qntInput.text, out int convertedQnt))
            {
                qnt = convertedQnt;
            }
            else
            {
                return;          
            }  
        }
        
        resourceAmount = ResourcesHandler.main.GetResource(resource);

        if (resourceAmount >= qnt)
        {
            resourceAmount -= qnt;
            ResourcesHandler.money += sellPrices[resource] * qnt;
            ResourcesHandler.main.UpdateResource(resource, resourceAmount);
        }
        else
        {
            StartCoroutine(HUDHandler.main.FlashFeedback());
        }
    }

}
