using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NegociationsHandler : MonoBehaviour
{

    [SerializeField] private TMP_Dropdown resourceDrop;
    [SerializeField] private TMP_InputField qntInput;

    [SerializeField] private ParticleSystem psSell, psSellAll;
    [SerializeField] private Animator capitalistAnimator;

    private int resourceAmount;

    private Dictionary<Resource, int> sellPrices = new Dictionary<Resource, int>()
    {
        {Resource.Carvão, 1},
        {Resource.Ferro, 3},
        {Resource.Petróleo, 10}
    };

    public void Sell(bool sellAll)
    {
        Resource resource = (Resource) resourceDrop.value;
        int qnt;
        ParticleSystem ps;

        if (sellAll)
        {
            qnt = ResourcesHandler.main.GetResource(resource);
            ps = psSellAll;
        }
        else
        {
            if (int.TryParse(qntInput.text, out int convertedQnt))
            {
                qnt = convertedQnt;
                ps = psSell;
            }
            else
            {
                return;          
            }  
        }
        
        resourceAmount = ResourcesHandler.main.GetResource(resource);

        if (resourceAmount >= qnt && qnt != 0)
        {
            resourceAmount -= qnt;
            ResourcesHandler.money += sellPrices[resource] * qnt;
            ResourcesHandler.main.UpdateResource(resource, resourceAmount);

            ps.Play();
            capitalistAnimator.SetTrigger("Sell");
        }
        else
        {
            StartCoroutine(HUDHandler.main.FlashFeedback());
        }
    }

}
