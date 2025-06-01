using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDHandler : MonoBehaviour
{
    public static HUDHandler main;

    [SerializeField] TextMeshProUGUI moneyLbl, coalLbl, ironLbl, oilLbl;
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
        coalLbl.text = "Carv�o: " + ResourcesHandler.coal;
        ironLbl.text = "Ferro: " + ResourcesHandler.iron;
        oilLbl.text = "Petr�leo: " + ResourcesHandler.oil;

        cO2Slider.value = ResourcesHandler.co2LvlIncreaser - ResourcesHandler.co2LvlDecreaser;
    }

    public void UpdateQuantities(Machine machine)
    {
        GameObject.Find(machine.name).transform.Find("Quantity").GetComponent<TextMeshProUGUI>().text = "x" + machine.qnt;
    }

    public void ShowEnergies(Machine machine)
    {
        GameObject.Find(machine.name).transform.Find("Toggles").gameObject.SetActive(true);
    }
}
