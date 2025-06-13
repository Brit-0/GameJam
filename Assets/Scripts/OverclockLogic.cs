using UnityEngine;
using UnityEngine.UI;

public class OverclockLogic : MonoBehaviour
{
    private Image currentLvlIcon;
    private ParticleSystem ps;
    
    private int currentLvl;

    Machine machine;
    
    [SerializeField] Sprite[] lvlIcons = new Sprite[4];

    private void Awake()
    {
        ps = transform.Find("EnergyParticle").GetComponent<ParticleSystem>();
        currentLvlIcon = transform.Find("CurrentLvlIcon").GetComponent<Image>();

        machine = (Machine)MachinesHandler.main.GetMachine(transform.parent.name);
        machine ??= (Machine)MachinesHandler.main.GetHelper(transform.parent.name);
    }

    public void IncreaseLevel()
    {
        if (ResourcesHandler.currentOverclocks > 0 && currentLvl < 3) //APLICAR PRÓXIMO NÍVEL
        {
            currentLvl++;
            ResourcesHandler.currentOverclocks--;
            machine.delay /= 2;
            ps.Play();
        }
        else //RESETAR NÍVEL
        {
            if (currentLvl == 0)
            {
                StartCoroutine(HUDHandler.main.FlashFeedback("Não possui overclocks o suficiente!"));
            }
            else
            {
                ResourcesHandler.currentOverclocks += currentLvl;
                machine.delay *= Mathf.Pow(2, currentLvl);
                currentLvl = 0;
                ps.Stop();
            }
        }

        HUDHandler.main.UpdateQuantities(machine);
        currentLvlIcon.sprite = lvlIcons[currentLvl];
    }
}
