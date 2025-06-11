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
    public int consumption;
    public int energyWaste;
    [Header("POLUIÇÃO")]
    public int co2Lvl;
    [Header("MÁQUINA GRANDE")]
    public bool isCombinable;
    public bool isBigMachine;
    [Header("REFERÊNCIAS")]
    public TileBase tile;
    public GameObject buyable;
    [Header("VARIÁVEIS")]
    public bool alreadyBought;
}


public class MachinesHandler : MonoBehaviour
{
    public static MachinesHandler main;

    [SerializeField] Tilemap tm;

    [SerializeField] Texture2D dinamiteIcon, axeIcon;

    [SerializeField] private List<Machine> allMachines;
    [SerializeField] private List<Machine> allHelpers;

    [HideInInspector]
    public Machine coalExtractor, oilExtractor, thermalPowerPlant, ironExtractor, smallMetalIndustry, oilPowerPlant, solarPanel, windmill;
    public Machine grandMetalIndustry, grandPowerPlant;
    [HideInInspector]
    public Machine smallReserve, grandReserve;
    private const int dinamitePrice = 50, axePrice = 30;

    private void Awake()
    {
        main = this;

        //MAQUINAS
        coalExtractor = (Machine)GetMachine("CoalExtractor");
        thermalPowerPlant = (Machine)GetMachine("ThermalPowerPlant");
        oilExtractor = (Machine)GetMachine("OilExtractor");
        ironExtractor = (Machine)GetMachine("IronExtractor");
        smallMetalIndustry = (Machine)GetMachine("SmallMetalIndustry");
        oilPowerPlant = (Machine)GetMachine("OilPowerPlant");
        solarPanel = (Machine)GetMachine("SolarPanel");
        windmill = (Machine)GetMachine("Windmill");

        grandMetalIndustry = (Machine)GetMachine("GrandMetalIndustry");
        grandPowerPlant = (Machine)GetMachine("GrandPowerPlant");

        //AUXILIARES
        smallReserve = (Machine)GetHelper("SmallReserve");
        grandReserve = (Machine)GetHelper("GrandReserve");
    }

    #region GET
    public object GetMachine(string machineName)
    {
        foreach (Machine machine in allMachines)
        {
            if (machineName.Contains(machine.name))
            {
                return machine;
            }
        }
        return null;
    }

    public object GetHelper(string helperName)
    {
        foreach (Machine helper in allHelpers)
        {
            if (helperName.Contains(helper.name))
            {
                return helper;
            }
        }
        return null;
    }

    #endregion

    #region BUYS

    public void BuyMachine(string machineName)
    {
        Machine machine = (Machine)GetMachine(machineName);

        if (ResourcesHandler.money >= machine.price) //COMPRA SUCEDIDA
        {
            AudioManager.PlayAudio(AudioManager.main.buy);
            ResourcesHandler.money -= machine.price;
            CameraController.main.StartCoroutine(CameraController.main.BlockPurchases());

            if (!machine.alreadyBought) 
            {
                machine.alreadyBought = true;

                StartCoroutine(machine.name + "Coroutine");

                HUDHandler.main.FirstPurchaseUpdate(machine);

                //ShowNextBuyable(); //MOSTRAR ELE

                if (machine.price == 0)
                {
                    machine.price = 30;
                }

                StartCoroutine(CameraController.main.ExpandViewport("Build", 1f));
            }
            else
            {
                if (machine.price < 2500)
                {
                    machine.price = Mathf.RoundToInt(machine.price * 1.5f / 10) * 10;
                }
                StartCoroutine(CameraController.main.ExpandViewport("Build", 0f));
            }

            HUDHandler.main.UpdatePrice(machine);
            HUDHandler.main.SpentMoneyParticle();
            
            TerrainLogic.currentTile = machine.tile;
        }
        else //COMPRA FRACASSADA
        {
            StartCoroutine(HUDHandler.main.FlashFeedback(CapitalistDialog.SelectDialog(CapitalistDialog.failedBuyMachine), 4f));
        }
    }

    public void BuyHelper(string helperName)
    {
        Machine helper = (Machine)GetHelper(helperName);

        if (ResourcesHandler.money >= helper.price)
        {
            AudioManager.PlayAudio(AudioManager.main.buy);
            ResourcesHandler.money -= helper.price;
            CameraController.main.StartCoroutine(CameraController.main.BlockPurchases());

            if (!helper.alreadyBought)
            {
                helper.alreadyBought = true;

                HUDHandler.main.FirstPurchaseUpdate(helper);

                StartCoroutine(CameraController.main.ExpandViewport("Build", 1f));
            }
            else
            {
                if (helper.price < 1500)
                {
                    helper.price = Mathf.RoundToInt(helper.price * 1.5f / 10) * 10;
                }
                StartCoroutine(CameraController.main.ExpandViewport("Build", 0f));
            }

            HUDHandler.main.UpdatePrice(helper);
            HUDHandler.main.SpentMoneyParticle();

            TerrainLogic.currentTile = helper.tile;
        }
        else
        {
            StartCoroutine(HUDHandler.main.FlashFeedback(CapitalistDialog.SelectDialog(CapitalistDialog.failedBuyHelper), 4f));
        }
    }

    public void BuyDinamite()
    {
        if (ResourcesHandler.money >= dinamitePrice)
        {
            ResourcesHandler.money -= dinamitePrice;

            TerrainLogic.isDemolishing = true;

            StartCoroutine(CameraController.main.ExpandViewport("Destroy", 0f));
            HUDHandler.main.SpentMoneyParticle();

            Cursor.SetCursor(dinamiteIcon, Vector2.zero, CursorMode.ForceSoftware);
        }
        else
        {
            StartCoroutine(HUDHandler.main.FlashFeedback("Precisa de mais " + (dinamitePrice - ResourcesHandler.money) + " reais", 2f)); 
        }
    }

    public void BuyAxe()
    {
        if (ResourcesHandler.money >= axePrice)
        {
            ResourcesHandler.money -= axePrice;

            TerrainLogic.isChopping = true;

            StartCoroutine(CameraController.main.ExpandViewport("Chop", 0f));
            HUDHandler.main.SpentMoneyParticle();

            Cursor.SetCursor(axeIcon, Vector2.zero, CursorMode.ForceSoftware);
        }
        else
        {
            StartCoroutine(HUDHandler.main.FlashFeedback("Precisa de mais " + (axePrice - ResourcesHandler.money) + " reais", 2f));
        }
    }

    #endregion

    #region Coroutines
    private IEnumerator CoalExtractorCoroutine()
    {
        yield return new WaitForSeconds(coalExtractor.delay);
        ResourcesHandler.coal += coalExtractor.qnt * coalExtractor.generation;

        StartCoroutine(CoalExtractorCoroutine());
    }

    private IEnumerator IronExtractorCoroutine()
    {
        yield return new WaitForSeconds(ironExtractor.delay);
        

        if (ResourcesHandler.energy >= ironExtractor.energyWaste * ironExtractor.qnt)
        {
            ResourcesHandler.energy -= ironExtractor.energyWaste * ironExtractor.qnt;
            ResourcesHandler.iron += ironExtractor.generation * ironExtractor.qnt;
        }
        else
        {
            StartCoroutine(HUDHandler.main.FlashError(ironExtractor));
        }

        StartCoroutine(IronExtractorCoroutine());
    }

    private IEnumerator OilExtractorCoroutine()
    {
        yield return new WaitForSeconds(oilExtractor.delay);

        if (ResourcesHandler.energy >= oilExtractor.energyWaste * oilExtractor.qnt)
        {
            ResourcesHandler.energy -= oilExtractor.energyWaste * oilExtractor.qnt;
            ResourcesHandler.oil += oilExtractor.generation * oilExtractor.qnt;
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

        if (ResourcesHandler.coal >= thermalPowerPlant.qnt * thermalPowerPlant.consumption)
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

    public IEnumerator GrandPowerPlantCoroutine()
    {
        yield return new WaitForSeconds(grandPowerPlant.delay);

        if (ResourcesHandler.coal >= grandPowerPlant.qnt * grandPowerPlant.consumption)
        {
            ResourcesHandler.coal -= grandPowerPlant.qnt * grandPowerPlant.consumption;
            ResourcesHandler.energy += grandPowerPlant.qnt * grandPowerPlant.generation;
        }
        else
        {
            StartCoroutine(HUDHandler.main.FlashError(grandPowerPlant));
        }

        StartCoroutine(GrandPowerPlantCoroutine());
    }

    private IEnumerator SmallMetalIndustryCoroutine()
    {
        yield return new WaitForSeconds(smallMetalIndustry.delay);

        if (ResourcesHandler.iron >= smallMetalIndustry.consumption * smallMetalIndustry.qnt && ResourcesHandler.energy >= smallMetalIndustry.energyWaste * smallMetalIndustry.qnt)
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

    public IEnumerator GrandMetalIndustryCoroutine()
    {
        yield return new WaitForSeconds(grandMetalIndustry.delay);

        if (ResourcesHandler.iron >= grandMetalIndustry.consumption * grandMetalIndustry.qnt && ResourcesHandler.energy >= grandMetalIndustry.energyWaste * grandMetalIndustry.qnt)
        {
            ResourcesHandler.iron -= grandMetalIndustry.consumption * grandMetalIndustry.qnt;
            ResourcesHandler.energy -= grandMetalIndustry.energyWaste * grandMetalIndustry.qnt;

            ResourcesHandler.ironBar += grandMetalIndustry.qnt * grandMetalIndustry.generation;
        }
        else
        {
            StartCoroutine(HUDHandler.main.FlashError(grandMetalIndustry));
        }

        StartCoroutine(GrandMetalIndustryCoroutine());
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

    private IEnumerator SolarPanelCoroutine()
    {
        yield return new WaitForSeconds(solarPanel.delay);

        ResourcesHandler.energy += solarPanel.generation * solarPanel.qnt;

        StartCoroutine(SolarPanelCoroutine());
    }

    private IEnumerator WindmillCoroutine()
    {
        yield return new WaitForSeconds(windmill.delay);

        ResourcesHandler.energy += windmill.generation * windmill.qnt;

        StartCoroutine(WindmillCoroutine());
    }

    #endregion


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
