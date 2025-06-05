using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;

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

    private void Update()
    {
        foreach (QuestData quest in activeQuests)
        {
            if (CheckIfCompleted(quest))
            {
                quest.isCompleted = true;
            }
        }

        if (questsToRemove.Count > 0)
        {
            activeQuests.Remove(questsToRemove[0]);
        }
        
    }

    [ContextMenu("ADICIONAR QUEST")]
    public void AddQuest()
    {
        if (activeQuests.Count >= 7) return;

        activeQuests.Add(availableQuests[Random.Range(0, availableQuests.Count)]); //SELECIONA UMA QUEST ALEATORIA
        QuestData quest = activeQuests[0]; //PEGA A PRIMEIRA QUEST ATIVA
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
