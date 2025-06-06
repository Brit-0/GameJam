using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;
using UnityEngine.UI;

public class QuestsManager : MonoBehaviour
{
    public static QuestsManager main;

    [SerializeField] private List<QuestData> availableQuests;
    [SerializeField] private List<QuestData> activeQuests;

    [Header("REFERENCIAS")]
    [SerializeField] private GameObject questBarPF;
    [SerializeField] private Transform limiter;

    private List<QuestData> questsToRemove = new();

    private void Awake()
    {
        main = this;
    }

    private void Start()
    {
        foreach (QuestData quest in availableQuests)
        {
            AddQuest(quest);
        }
    }

    private void Update()
    {
        foreach (QuestData quest in activeQuests)
        {
            if (quest.isCompleted) continue;

            if (CheckIfCompleted(quest))
            {
                quest.isCompleted = true;
                quest.questBar.GetComponent<Button>().interactable = true;
            }
        }

        if (questsToRemove.Count > 0)
        {
            activeQuests.Remove(questsToRemove[0]);
        }
        
    }

    [ContextMenu("ADICIONAR QUEST")]
    public void AddQuest(QuestData quest)
    {
        if (activeQuests.Count >= 7) return;

        activeQuests.Add(quest); 
        GameObject questBar = Instantiate(questBarPF, limiter);
        quest.questBar = questBar;
        questBar.GetComponent<QuestBar>().SetBar(quest);
    }

    private bool CheckIfCompleted(QuestData quest)
    {
        switch (quest.type)
        {
            case QuestType.CollectResources:
                if (ResourcesHandler.main.GetResource(quest.resourceQuest.resourceNeeded) >= quest.resourceQuest.neededAmount)
                {
                    return true;
                }

                break;

            case QuestType.HaveMachines:
                Machine machine = (Machine)MachinesHandler.main.GetMachine(quest.machineQuest.machineNeededName);

                if (machine.qnt >= quest.machineQuest.neededAmount)
                {
                    return true;
                }

                break;

            case QuestType.ReachMoney:
                if (ResourcesHandler.money >= quest.moneyQuest.moneyAmount)
                {
                    return true;
                }

                break;
        }

        return false;
    }

    public void CompleteQuest(QuestData quest)
    {
        switch (quest.rewardType)
        {
            case QuestRewardType.Money:
                ResourcesHandler.money += quest.moneyReward.moneyAmount;

                break;

            case QuestRewardType.MachineUnlock:
                Machine machine = (Machine)MachinesHandler.main.GetMachine(quest.machineReward.machineName);
                machine.buyable.SetActive(true);
                break;
        }

        questsToRemove.Add(quest);
    }
}
