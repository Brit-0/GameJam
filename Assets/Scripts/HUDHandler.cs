using System.Collections;
using System.Runtime.InteropServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HUDHandler : MonoBehaviour
{
    public static HUDHandler main;

    [SerializeField] TextMeshProUGUI moneyLbl, coalLbl, ironLbl, oilLbl, energyLbl;
    [SerializeField] TextMeshProUGUI feedbackLbl;
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
        GameObject.Find(machine.name).transform.Find("New/Quantity").GetComponent<TextMeshProUGUI>().text = "x" + machine.qnt;
    }

    public void UpdatePrice(Machine machine)
    {
        GameObject.Find(machine.name).transform.Find("New/NewBuyBtn/BtnLbl").GetComponent<TextMeshProUGUI>().text = "$" + machine.price;
    }

    public IEnumerator FlashError(Machine machine)
    {
        GameObject errorIcon = GameObject.Find(machine.name).transform.Find("New/ErrorIcon").gameObject; 
    
        errorIcon.SetActive(true);

        yield return new WaitForSeconds(1f);

        errorIcon.SetActive(false);
    }

    public IEnumerator FlashFeedback()
    {
        feedbackLbl.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        feedbackLbl.gameObject.SetActive(false);
    }

    public void FirstPurchaseUpdate(Machine machine)
    {
        GameObject.Find(machine.name).transform.Find("First").gameObject.SetActive(false);
        GameObject.Find(machine.name).transform.Find("New").gameObject.SetActive(true);
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
