using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HUDHandler : MonoBehaviour
{
    public static HUDHandler main;

    [Header("PAGES")]
    [SerializeField] List<GameObject> pages;
    [SerializeField] GameObject pagesBtns;
    [Header("LABELS")]
    [SerializeField] TextMeshProUGUI moneyLbl;
    [SerializeField] TextMeshProUGUI coalLbl;
    [SerializeField] TextMeshProUGUI ironLbl;
    [SerializeField] TextMeshProUGUI oilLbl;
    [SerializeField] TextMeshProUGUI energyLbl;
    [SerializeField] TextMeshProUGUI ironBarLbl;
    [SerializeField] TextMeshProUGUI totalEnergyLbl;
    [SerializeField] GameObject feedbackPanel, storeFeedbackPanel;
    [Header("SLIDER")]
    [SerializeField] Slider cO2Slider;
    [Header("OVERLAY")]
    [SerializeField] GameObject heatOverlay;
    [Header("PARTICLES")]
    [SerializeField] ParticleSystem moneyParticle2;
    [Header("CAPITALIST")]
    [SerializeField] Animator capitalistAnimator;

    private int currentPage = 1;
    private float currentCO2, currentVelocity = 0;

    private void Awake()
    {
        main = this;
    }

    private void Update()
    {
        UpdateResources();

        if (cO2Slider.value > 9.5f)
        {
            heatOverlay.GetComponent<UITweener>().AlphaFlick();

            if (!TerrainLogic.main.isTornadoing)
            {
                TerrainLogic.main.Tornado();
            }
        }
    }

    #region LBLSUPDATES

    private void UpdateResources()
    {
        moneyLbl.text = ResourcesHandler.money.ToString();
        coalLbl.text = ResourcesHandler.coal.ToString();
        ironLbl.text = ResourcesHandler.iron.ToString();
        oilLbl.text = ResourcesHandler.oil.ToString();
        ironBarLbl.text = ResourcesHandler.ironBar.ToString();

        energyLbl.text = ResourcesHandler.energy.ToString();

        currentCO2 = Mathf.SmoothDamp(cO2Slider.value, ResourcesHandler.co2Lvl, ref currentVelocity, 100 * Time.deltaTime);
        cO2Slider.value = currentCO2;   
    }

    public void UpdateQuantities(Machine machine)
    {
        machine.buyable.transform.Find("Quantity/QuantityLbl").GetComponent<TextMeshProUGUI>().text = "x" + machine.qnt;
        UpdateTotalEnergy();
    }

    public void UpdatePrice(Machine machine)
    {
        machine.buyable.transform.Find("Price/PriceLbl").GetComponent<TextMeshProUGUI>().text = machine.price.ToString();
    }

    public void UpdateTotalEnergy()
    {
        float totalEnergy = 0f;

        totalEnergy += Mathf.Round(MachinesHandler.main.thermalPowerPlant.generation * MachinesHandler.main.thermalPowerPlant.qnt) / Mathf.Round(MachinesHandler.main.thermalPowerPlant.delay);
        totalEnergy += Mathf.Round(MachinesHandler.main.oilPowerPlant.generation * MachinesHandler.main.oilPowerPlant.qnt) / Mathf.Round(MachinesHandler.main.oilPowerPlant.delay);
        totalEnergy += Mathf.Round(MachinesHandler.main.solarPanel.generation * MachinesHandler.main.solarPanel.qnt) / Mathf.Round(MachinesHandler.main.solarPanel.delay);
        totalEnergy += Mathf.Round(MachinesHandler.main.windmill.generation * MachinesHandler.main.windmill.qnt) / Mathf.Round(MachinesHandler.main.windmill.delay);

        totalEnergyLbl.text = Regex.Replace(totalEnergyLbl.text, "[-+]?\\d+(\\,\\d+)?", totalEnergy.ToString("0.0"));
    }

    #endregion

    #region CAPITALIST

    public void CapitalistSmile()
    {
        capitalistAnimator.SetTrigger("Smile");
        StartCoroutine(FlashFeedback("Está querendo me dizer alguma coisa?", 2f));
    }

    public void CapitalistDemon(bool active)
    {
        capitalistAnimator.SetBool("Demon", active);
    }

    #endregion

    #region FLASHCOROUTINES

    public IEnumerator FlashError(Machine machine)
    {
        GameObject errorIcon = machine.buyable.transform.Find("ErrorIcon").gameObject;

        if (errorIcon.activeInHierarchy) yield break;
    
        errorIcon.SetActive(true);

        yield return new WaitForSeconds(1f);

        errorIcon.SetActive(false);
    }

    public IEnumerator FlashStoreFeedback()
    {
        if (!storeFeedbackPanel.activeInHierarchy)
        {
            storeFeedbackPanel.SetActive(true);

            yield return new WaitForSeconds(2f);

            storeFeedbackPanel.SetActive(false);
        }
    }

    public IEnumerator FlashFeedback(string feedback, float duration = 2f)
    {
        if (feedbackPanel.activeInHierarchy) yield break;

        feedbackPanel.transform.Find("FBLbl").GetComponent<TextMeshProUGUI>().text = feedback;

        feedbackPanel.SetActive(true);

        yield return new WaitForSeconds(duration);

        feedbackPanel.SetActive(false);
    }

    #endregion

    #region HUDUPDATES
    public void FirstPurchaseUpdate(Machine machine)
    {
        LeanTween.color(machine.buyable.transform.Find("Button/MachineIcon").GetComponent<RectTransform>(), Color.white, 1f);
        machine.buyable.transform.Find("Quantity").gameObject.SetActive(true);
        machine.buyable.transform.Find("Tip").gameObject.SetActive(true);
    }

    public void ChangePage(bool next)
    {
        pages[currentPage - 1].SetActive(false);

        if (next && currentPage < pages.Count)
        {           
            currentPage++;
        }
        else if (!next && currentPage > 1)
        {
            currentPage--;
        }

        pages[currentPage - 1].SetActive(true);
    }

    public void UnlockPage()
    {
        pagesBtns.SetActive(true);
    }

    #endregion

    public void SpentMoneyParticle()
    {
        moneyParticle2.Play();
    }
 
}
