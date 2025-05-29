using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[System.Serializable]
public class Machine
{
    public string name;
    public int price;
    public int qnt;
}


public class MachinesHandler : MonoBehaviour
{
    //private Machine CoalExtractor = new Machine() { name = "CoalExtractor", price = 30};

    [SerializeField] private List<Machine> allMachines = new List<Machine>();

    private Machine coalExtractor;

    private void Awake()
    {
        coalExtractor = (Machine)GetMachine("CoalExtractor");
    }

    private object GetMachine(string machineName)
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

        if (ResourcesHandler.main.Money >= machine.price)
        {
            ResourcesHandler.main.Money -= machine.price;
            machine.qnt += 1;

            if (machine.qnt == 1)
            {
                StartCoroutine(machine.name + "Coroutine");
            }

            print("Comprou máquina");
        }
        else
        {
            print("Precisa de mais " + (machine.price - ResourcesHandler.main.Money) + " reais");
        }
        
    }

    private IEnumerator CoalExtractorCoroutine()
    {
        yield return new WaitForSeconds(2f);
        ResourcesHandler.main.Coal += coalExtractor.qnt;

        StartCoroutine(CoalExtractorCoroutine());
    }
}
