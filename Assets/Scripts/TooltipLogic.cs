using System.Text.RegularExpressions;
using TMPro;
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
        CtipLbl.text = Regex.Replace(CtipLbl.text, "[-+]?\\d+(\\,\\d+)?", (Mathf.Round(machine.consumption) / Mathf.Round(machine.delay) * machine.qnt).ToString("0.0"));
        tipPanel.SetActive(true);

        TextMeshProUGUI GtipLbl = tipPanel.transform.Find("GTipLbl").GetComponent<TextMeshProUGUI>();  
        GtipLbl.text = Regex.Replace(GtipLbl.text, "[-+]?\\d+(\\,\\d+)?", (Mathf.Round(machine.generation) / Mathf.Round(machine.delay) * machine.qnt).ToString("0.0"));
        tipPanel.SetActive(true);
    }
}
