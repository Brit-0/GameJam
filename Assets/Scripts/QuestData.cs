using System;
using UnityEngine;

public enum QuestType
{
    CollectResources,
    HaveMachines,
    ReachMoney
}

public enum QuestRewardType
{
    Money,
    MachineUnlock,
    Boost
}

[System.Serializable]
public class QuestData
{
    [Header("PRINCIPAL")]
    public string description;
    public bool isCompleted;
    public QuestType type;
    public QuestRewardType rewardType;
    public GameObject questBar;

    [Header("POR TIPO")]
    public ResourceQuest resourceQuest;
    public MachineQuest machineQuest;
    public MoneyQuest moneyQuest;

    [Header("POR RECOMPENSA")]
    public MoneyReward moneyReward;
    public MachineReward machineReward;

    //TIPOS

    [Serializable]
    public struct ResourceQuest
    {
        public Resource resourceNeeded;
        public int neededAmount;
    }

    [Serializable]
    public struct MachineQuest
    {
        public string machineNeededName;
        public int neededAmount;
    }

    [Serializable]
    public struct MoneyQuest
    {
        public int moneyAmount;
    }

    //RECOMPENSAS

    [Serializable]
    public struct MoneyReward
    {
        public int moneyAmount;
    }

    [Serializable]
    public struct MachineReward
    {
        public string machineName;
    }
}
