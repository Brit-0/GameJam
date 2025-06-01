using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDHandler : MonoBehaviour
{
    public static HUDHandler main;

    [SerializeField] TextMeshProUGUI moneyLbl, coalLbl, ironLbl, oilLbl, energyLbl;
    [SerializeField] Slider cO2Slider;

    private void Awake()
    {
        main = this;
    }

    private void Update()
    {
        UpdateResources();
    }

    private void UpdateResources()
    {
        moneyLbl.text = "Dinheiro: " + ResourcesHandler.money;
        coalLbl.text = "Carvão: " + ResourcesHandler.coal;
        ironLbl.text = "Ferro: " + ResourcesHandler.iron;
        oilLbl.text = "Petróleo: " + ResourcesHandler.oil;

        energyLbl.text = "Energia: " + ResourcesHandler.energy;

        cO2Slider.value = ResourcesHandler.co2LvlIncreaser - ResourcesHandler.co2LvlDecreaser;
    }

    public void UpdateQuantities(Machine machine)
    {
        GameObject.Find(machine.name).transform.Find("Quantity").GetComponent<TextMeshProUGUI>().text = "x" + machine.qnt;
    }

    public void ShowEnergies(Machine machine)
    {
        GameObject toggles = GameObject.Find(machine.name).transform.Find("Toggles").gameObject;

        if (toggles)
        {
            toggles.SetActive(true);
        }
    }
}
