using TMPro;
using UnityEngine;

public class HUDHandler : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI moneyLbl, coalLbl;

    public void updateResources()
    {
        moneyLbl.text = "Dinheiro: " + ResourcesHandler.main.Money;
        coalLbl.text = "Carv�o: " + ResourcesHandler.main.Coal;
    }
}
