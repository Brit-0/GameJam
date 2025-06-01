using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

[System.Serializable]
public class Machine
{
    public string name;
    public int price;
    public int qnt;
    public int delay;
    public int energyWaste;
    public int co2Lvl;
    public Tile tile;
    public GameObject buyable;
}

public class MachinesHandler : MonoBehaviour
{
    public static MachinesHandler main;

    [SerializeField] private List<Machine> allMachines;

    private int currentBuyableIndex;
    private Machine coalExtractor, oilExtractor, thermalPowerPlant;

    private void Awake()
    {
        main = this;

        coalExtractor = (Machine)GetMachine("CoalExtractor");
        thermalPowerPlant = (Machine)GetMachine("ThermalPowerPlant");
        oilExtractor = (Machine)GetMachine("OilExtractor");    
    }

    public object GetMachine(string machineName)
    {
        foreach (Machine machine in allMachines)
        {
            if (machine.name == machineName)
            {
                return machine;
            }
        }
        return null;
    }

    public void BuyMachine(string machineName)
    {
        Machine machine = (Machine)GetMachine(machineName);

        if (ResourcesHandler.money >= machine.price)
        {
            ResourcesHandler.money -= machine.price;

            if (machine.qnt == 0)
            {
                StartCoroutine(machine.name + "Coroutine");
                //HUDHandler.main.ShowEnergies(machine);
                currentBuyableIndex++;
                ShowNextBuyable();
            }

            print("Comprou M�quina");

            CameraController.main.ExpandViewport("Build");
            TerrainLogic.currentTile = machine.tile;
        }
        else
        {
            print("Precisa de mais " + (machine.price - ResourcesHandler.money) + " reais");
        }
        
    }

    public void BuyDinamite()
    {
        if (ResourcesHandler.money >= 50)
        {
            ResourcesHandler.money -= 50;
            print("Comprou Dinamite");

            TerrainLogic.isDestroying = true;

            CameraController.main.ExpandViewport("Destroy");
        }
        else
        {
            print("Precisa de mais " + (50 - ResourcesHandler.money) + " reais");
        }
    }

    private void ShowNextBuyable()
    {
        if (currentBuyableIndex >= allMachines.Count) return;

        allMachines[currentBuyableIndex].buyable.SetActive(true);
    }

    private IEnumerator CoalExtractorCoroutine()
    {
        yield return new WaitForSeconds(coalExtractor.delay);
        ResourcesHandler.coal += coalExtractor.qnt;

        StartCoroutine(CoalExtractorCoroutine());
    }

    private IEnumerator OilExtractorCoroutine()
    {
        yield return new WaitForSeconds(oilExtractor.delay);
        ResourcesHandler.oil += oilExtractor.qnt;
        ResourcesHandler.energy -= oilExtractor.energyWaste * oilExtractor.qnt;

        StartCoroutine(OilExtractorCoroutine());
    }

    private IEnumerator ThermalPowerPlantCoroutine()
    {
        yield return new WaitForSeconds(thermalPowerPlant.delay);

        if (ResourcesHandler.coal > 0)
        {
            ResourcesHandler.coal--;
            ResourcesHandler.energy++;
        }

        StartCoroutine(ThermalPowerPlantCoroutine());
    }
}
