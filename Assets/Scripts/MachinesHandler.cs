using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class Machine
{
    [Header("PRINCIPAL")]
    public string name;
    public int price;
    public int qnt;
    [Header("GERAÇÃO")]
    public int generation;
    public int delay;
    [Header("CONSUMO")]
    public Resource resourceConsumed;
    public int consumption;
    public int energyWaste;
    [Header("POLUIÇÃO")]
    public int co2Lvl;
    [Header("REFERÊNCIAS")]
    public TileBase tile;
    public GameObject buyable;
}

public class MachinesHandler : MonoBehaviour
{
    public static MachinesHandler main;

    [SerializeField] private List<Machine> allMachines, allHelpers;

    private int currentBuyableIndex, currentHelperIndex;
    private Machine coalExtractor, oilExtractor, thermalPowerPlant, ironExtractor, smallMetalIndustry, oilPowerPlant;
    
    

    private void Awake()
    {
        main = this;

        coalExtractor = (Machine)GetMachine("CoalExtractor");
        thermalPowerPlant = (Machine)GetMachine("ThermalPowerPlant");
        oilExtractor = (Machine)GetMachine("OilExtractor");
        ironExtractor = (Machine)GetMachine("IronExtractor");
        smallMetalIndustry = (Machine)GetMachine("SmallMetalIndustry");
        oilPowerPlant = (Machine)GetMachine("OilPowerPlant");
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

    public object GetHelper(string machineName)
    {
        foreach (Machine machine in allHelpers)
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

                HUDHandler.main.FirstPurchaseUpdate(machine);

                currentBuyableIndex++; //PEGAR PRÓXIMO DESBLOQUEÁVEL E 
                //ShowNextBuyable(); //MOSTRAR ELE

                if (machine.price == 0)
                {
                    machine.price = 30;
                }
            }
            else
            {
                machine.price = Mathf.RoundToInt(machine.price * 1.5f / 10) * 10;
            }

            HUDHandler.main.UpdatePrice(machine);
            HUDHandler.main.SpentMoneyParticle();
            StartCoroutine(CameraController.main.ExpandViewport("Build"));
            TerrainLogic.currentTile = machine.tile;
        }
        else
        {
            print("Precisa de mais " + (machine.price - ResourcesHandler.money) + " reais");
        }
    }

    public void BuyHelper(string machineName)
    {
        Machine machine = (Machine)GetMachine(machineName);

        if (ResourcesHandler.money >= machine.price)
        {
            ResourcesHandler.money -= machine.price;

            if (machine.qnt == 0)
            {
                HUDHandler.main.FirstPurchaseUpdate(machine);

                currentHelperIndex++; 
            }
            else
            {
                machine.price = Mathf.RoundToInt(machine.price * 1.5f / 10) * 10;
            }

            HUDHandler.main.UpdatePrice(machine);
            HUDHandler.main.SpentMoneyParticle();
            StartCoroutine(CameraController.main.ExpandViewport("Build"));
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
            HUDHandler.main.SpentMoneyParticle();
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

    private IEnumerator IronExtractorCoroutine()
    {
        yield return new WaitForSeconds(ironExtractor.delay);
        ResourcesHandler.iron += ironExtractor.qnt * ironExtractor.generation;

        StartCoroutine(IronExtractorCoroutine());
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

    private IEnumerator SmallMetalIndustryCoroutine()
    {
        yield return new WaitForSeconds(smallMetalIndustry.delay);

        if (ResourcesHandler.iron >= smallMetalIndustry.consumption * smallMetalIndustry.qnt)
        {
            ResourcesHandler.iron -= smallMetalIndustry.consumption * smallMetalIndustry.qnt;
            ResourcesHandler.energy -= smallMetalIndustry.energyWaste * smallMetalIndustry.qnt;

            ResourcesHandler.ironBar += smallMetalIndustry.qnt * smallMetalIndustry.generation;
        }
        else
        {
            StartCoroutine(HUDHandler.main.FlashError(smallMetalIndustry));
        }

        StartCoroutine(SmallMetalIndustryCoroutine());
    }

    private IEnumerator OilPowerPlantCoroutine()
    {
        yield return new WaitForSeconds(oilPowerPlant.delay);

        if (ResourcesHandler.oil >= oilPowerPlant.consumption * oilPowerPlant.qnt)
        {
            ResourcesHandler.oil -= oilPowerPlant.consumption * oilPowerPlant.qnt;
            ResourcesHandler.energy += oilPowerPlant.generation * oilPowerPlant.qnt;
        }
        else
        {
            StartCoroutine(HUDHandler.main.FlashError(oilPowerPlant));
        }

        StartCoroutine(OilPowerPlantCoroutine());
    }

    public Machine GetMachineFromResource(Resource resource)
    {
        switch (resource)
        {
            case Resource.Carvão: return coalExtractor;
            case Resource.Ferro: return null;
            case Resource.Petróleo: return oilExtractor;
            case Resource.Energia: return thermalPowerPlant;
        }

        return null;
    }
}
