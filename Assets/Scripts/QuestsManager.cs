using System.Collections;
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
    [SerializeField] private GameObject questLbl;
    [SerializeField] GameObject questExclamationIcon;

    private List<QuestData> questsToRemove = new();
    private List<QuestData> questsToAdd = new();

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
                questLbl.SetActive(true);
                questExclamationIcon.SetActive(true);
            }
        }

        if (questsToRemove.Count > 0)
        {
            activeQuests.Remove(questsToRemove[0]);
            questsToRemove.Remove(questsToRemove[0]);
        }

        if (questsToAdd.Count > 0)
        {
            if (AddQuest(questsToAdd[0]))
            {
                questsToAdd.Remove(questsToAdd[0]);
            }
        }

        CompletedFeedback();
    }

    private QuestData GetQuest(string description)
    {
        foreach (QuestData quest in availableQuests)
        {
            if (quest.description == description)
            {
                return quest;
            }
        }

        return null;
    }

    public bool AddQuest(QuestData quest)
    {
        if (activeQuests.Count >= 7) return false;

        activeQuests.Add(quest); 
        GameObject questBar = Instantiate(questBarPF, limiter);
        quest.questBar = questBar;
        questBar.GetComponent<QuestBar>().SetBar(quest);

        return true;
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
                machine ??= (Machine)MachinesHandler.main.GetHelper(quest.machineQuest.machineNeededName);

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
                AudioManager.PlayAudio(AudioManager.main.sell);

                break;

            case QuestRewardType.MachineUnlock:
                Machine machine = (Machine)MachinesHandler.main.GetMachine(quest.machineReward.machineName);
                machine.buyable.SetActive(true);
                StartCoroutine(HUDHandler.main.FlashStoreFeedback("Desbloqueou uma máquina nova!"));

                break;
        }

        if (quest.hasSequencialQuest)
        {
            questsToAdd.Add(GetQuest(quest.nextQuest));
        }

        questsToRemove.Add(quest);
    }

    private void CompletedFeedback()
    {
        bool hasCompleted = false;

        foreach (QuestData quest in activeQuests)
        {
            if (quest.isCompleted)
            {
                hasCompleted = true;
                break;
            }
        }

        if (!hasCompleted)
        {
            questLbl.SetActive(false);
            questExclamationIcon.SetActive(false);
        }
    }
}
