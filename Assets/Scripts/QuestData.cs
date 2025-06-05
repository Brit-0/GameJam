using System;
using UnityEngine;

public enum QuestType
{
    CollectResources,
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
