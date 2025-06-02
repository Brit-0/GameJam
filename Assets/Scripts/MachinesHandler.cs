using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

[System.Serializable]
public class Machine
{
    [Header("PRINCIPAL")]
    public string name;
    public int price;
    public int qnt;
    [Header("GERA��O")]
    public int generation;
    public int delay;
    [Header("CONSUMO")]
    public Resource resourceConsumed;
    public int consumption;
    public int energyWaste;
    [Header("POLUI��O")]
    public int co2Lvl;
    [Header("REFER�NCIAS")]
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

                HUDHandler.main.FirstPurchaseUpdate(machine);

                currentBuyableIndex++; //PEGAR PR�XIMO DESBLOQUE�VEL E 
                ShowNextBuyable(); //MOSTRAR ELE

                if (machine.price == 0)
                {
                    machine.price = 30;
                }
            }
            else
            {
                machine.price = Mathf.RoundToInt(machine.price * 1.5f / 10) * 10;
                HUDHandler.main.UpdatePrice(machine);
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
        ResourcesHandler.coal += coalExtractor.qnt * coalExtractor.generation;

        StartCoroutine(CoalExtractorCoroutine());
    }

    private IEnumerator OilExtractorCoroutine()
    {
        yield return new WaitForSeconds(oilExtractor.delay);

        if (ResourcesHandler.energy >= oilExtractor.energyWaste * oilExtractor.qnt)
        {
            ResourcesHandler.energy -= oilExtractor.energyWaste * oilExtractor.qnt;
            ResourcesHandler.oil += oilExtractor.generation;
        }
        else
        {
            StartCoroutine(HUDHandler.main.FlashError(oilExtractor));
        }

            StartCoroutine(OilExtractorCoroutine());
    }

    private IEnumerator ThermalPowerPlantCoroutine()
    {
        yield return new WaitForSeconds(thermalPowerPlant.delay);

        if (ResourcesHandler.coal >= thermalPowerPlant.qnt)
        {
            ResourcesHandler.coal -= thermalPowerPlant.qnt * thermalPowerPlant.consumption;
            ResourcesHandler.energy += thermalPowerPlant.qnt * thermalPowerPlant.generation;
        }
        else
        {
            StartCoroutine(HUDHandler.main.FlashError(thermalPowerPlant));
        }

        StartCoroutine(ThermalPowerPlantCoroutine());
    }

    public Machine GetMachineFromResource(Resource resource)
    {
        switch (resource)
        {
            case Resource.Carv�o: return coalExtractor;
            case Resource.Ferro: return null;
            case Resource.Petr�leo: return oilExtractor;
            case Resource.Energia: return thermalPowerPlant;
        }

        return null;
    }
}
