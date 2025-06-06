using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDHandler : MonoBehaviour
{
    public static HUDHandler main;

    [Header("LABELS")]
    [SerializeField] TextMeshProUGUI moneyLbl, coalLbl, ironLbl, oilLbl, energyLbl, ironBarLbl;
    [SerializeField] GameObject feedbackPanel, storeFeedbackPanel;
    [Header("SLIDER")]
    [SerializeField] Slider cO2Slider;
    [Header("OVERLAY")]
    [SerializeField] GameObject heatOverlay;
    [Header("PARTICLES")]
    [SerializeField] ParticleSystem moneyParticle2;
    [Header("CAPITALIST")]
    [SerializeField] Animator capitalistAnimator;

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

    private void UpdateResources()
    {
        moneyLbl.text = ResourcesHandler.money.ToString();
        coalLbl.text = ResourcesHandler.coal.ToString();
        ironLbl.text = ResourcesHandler.iron.ToString();
        oilLbl.text = ResourcesHandler.oil.ToString();
        ironBarLbl.text = ResourcesHandler.ironBar.ToString();

        energyLbl.text = ResourcesHandler.energy.ToString();

        currentCO2 = Mathf.SmoothDamp(cO2Slider.value, ResourcesHandler.co2LvlIncreaser - ResourcesHandler.co2LvlDecreaser, ref currentVelocity, 100 * Time.deltaTime);
        cO2Slider.value = currentCO2;
    }

    public void UpdateQuantities(Machine machine)
    {
        machine.buyable.transform.Find("Quantity/QuantityLbl").GetComponent<TextMeshProUGUI>().text = "x" + machine.qnt;
    }

    public void UpdatePrice(Machine machine)
    {
        machine.buyable.transform.Find("Price/PriceLbl").GetComponent<TextMeshProUGUI>().text = machine.price.ToString();
    }

    public void SpentMoneyParticle()
    {
        moneyParticle2.Play();
    }

    public void CapitalistSmile()
    {
        capitalistAnimator.SetTrigger("Smile");
        StartCoroutine(FlashFeedback("Está querendo me dizer alguma coisa?", 2f));
    }

    public void CapitalistDemon(bool active)
    {
        capitalistAnimator.SetBool("Demon", active);
    }

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
        if (storeFeedbackPanel.activeInHierarchy) yield break;

        storeFeedbackPanel.SetActive(true);

        yield return new WaitForSeconds(2f);

        storeFeedbackPanel.SetActive(false);
    }

    public IEnumerator FlashFeedback(string feedback, float duration)
    {
        if (feedbackPanel.activeInHierarchy) yield break;

        feedbackPanel.transform.Find("FBLbl").GetComponent<TextMeshProUGUI>().text = feedback;

        feedbackPanel.SetActive(true);

        yield return new WaitForSeconds(duration);

        feedbackPanel.SetActive(false);
    }


    public void FirstPurchaseUpdate(Machine machine)
    {
        LeanTween.color(machine.buyable.transform.Find("Button/MachineIcon").GetComponent<RectTransform>(), Color.white, 1f);
        machine.buyable.transform.Find("Quantity").gameObject.SetActive(true);
        machine.buyable.transform.Find("Tip").gameObject.SetActive(true);
    }
}
