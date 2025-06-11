using System.Text.RegularExpressions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TooltipLogic : MonoBehaviour
{
    private GameObject tipPanel;
    [SerializeField] private string machineName;

    private void Awake()
    {
        tipPanel = transform.Find("TipPanel").gameObject;
    }

    public void ShowTooltip()
    {
        Machine machine = (Machine)MachinesHandler.main.GetMachine(machineName);

        TextMeshProUGUI CtipLbl = tipPanel.transform.Find("CTipLbl").GetComponent<TextMeshProUGUI>();
        CtipLbl.text = Regex.Replace(CtipLbl.text, @"\b\d+\.\d+|\b\d+,\d+\b", (Mathf.Round(machine.consumption) / Mathf.Round(machine.delay) * machine.qnt).ToString("0.0"));

        TextMeshProUGUI EtipLbl = tipPanel.transform.Find("ETipLbl").GetComponent<TextMeshProUGUI>();
        EtipLbl.text = Regex.Replace(EtipLbl.text, @"\b\d+\.\d+|\b\d+,\d+\b", (Mathf.Round(machine.energyWaste) / Mathf.Round(machine.delay) * machine.qnt).ToString("0.0"));


        TextMeshProUGUI GtipLbl = tipPanel.transform.Find("GTipLbl").GetComponent<TextMeshProUGUI>();  
        GtipLbl.text = Regex.Replace(GtipLbl.text, @"\b\d+\.\d+|\b\d+,\d+\b", (Mathf.Round(machine.generation) / Mathf.Round(machine.delay) * machine.qnt).ToString("0.0"));


        tipPanel.SetActive(true);
    }

    public void ShowHelperTooltip()
    {
        Machine helper = (Machine)MachinesHandler.main.GetHelper(machineName);
        TextMeshProUGUI tipLbl = tipPanel.transform.Find("TipLbl").GetComponent<TextMeshProUGUI>();
        tipLbl.text = Regex.Replace(tipLbl.text, @"\b\d+\.\d+|\b\d+,\d+\b", (helper.co2Lvl * -1 * helper.qnt).ToString("0.0"));

        tipPanel.SetActive(true);
    }
}
