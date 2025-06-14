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
        {Resource.Carv�o, 1},
        {Resource.Ferro, 3},
        {Resource.Petr�leo, 10},
        {Resource.BarraDeFerro, 15 }
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
            AudioManager.PlayAudio(AudioManager.main.sell, .7f);

            if (sellPrices[resource] * qnt > 600)
            {
                capitalistAnimator.SetTrigger("Sell2");
            }
            else
            {
                capitalistAnimator.SetTrigger("Sell");
            }
        }
        else
        {
            StartCoroutine(HUDHandler.main.FlashStoreFeedback("N�o possui o suficiente desse recurso!"));
        }
    }

}
