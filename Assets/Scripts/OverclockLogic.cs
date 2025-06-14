using UnityEngine;
using UnityEngine.UI;

public class OverclockLogic : MonoBehaviour
{
    private Image currentLvlIcon;
    private ParticleSystem ps;
    
    private int currentLvl;

    Machine machine;

    [SerializeField] OverclockType type;
    
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
        int current;

        if (type == OverclockType.Resource)
        {
            current = ResourcesHandler.currentROverclocks;
        }
        else
        {
            current = ResourcesHandler.currentEOverclocks;
        }

        if (current > 0 && currentLvl < 3) //APLICAR PR�XIMO N�VEL
        {
            currentLvl++;

            if(type == OverclockType.Resource)
            {
                ResourcesHandler.currentROverclocks--;
            }
            else
            {
                ResourcesHandler.currentEOverclocks--;
            }

            machine.delay /= 2;
            ps.Play();
        }
        else //RESETAR N�VEL
        {
            if (currentLvl == 0)
            {
                StartCoroutine(HUDHandler.main.FlashFeedback("N�o possui overclocks suficientes desse tipo!"));
            }
            else
            {
                if (type == OverclockType.Resource)
                {
                    ResourcesHandler.currentROverclocks += currentLvl;
                }
                else
                {
                    ResourcesHandler.currentEOverclocks += currentLvl;
                }

                machine.delay *= Mathf.Pow(2, currentLvl);
                currentLvl = 0;
                ps.Stop();
            }
        }

        HUDHandler.main.UpdateQuantities(machine);
        currentLvlIcon.sprite = lvlIcons[currentLvl];
    }
}
