using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestBar : MonoBehaviour
{  
    [SerializeField] private TextMeshProUGUI description, progress;
    private QuestData quest;
    private bool wasColorSet;

    public void SetBar(QuestData quest)
    {
        this.quest = quest;

        description.text = quest.description;
        DefineProgress();
    }

    public void CallQuestCompleted()
    {
        if (!quest.isCompleted) return;

        QuestsManager.main.CompleteQuest(quest);
        Destroy(gameObject);
    }

    private void Update()
    {
        if (quest.isCompleted && !wasColorSet)
        {
            wasColorSet = true;
            transform.GetComponent<Image>().color = new Color32(169, 255, 162, 255);
        }
        else
        {
            DefineProgress();
        }
    }

    private void DefineProgress()
    {
        switch (quest.type) 
        {
            case QuestType.CollectResources:
                progress.text = ResourcesHandler.main.GetResource(quest.resourceQuest.resourceNeeded) + "/" + quest.resourceQuest.neededAmount;

                break;

            case QuestType.HaveMachines:
                Machine machine = (Machine)MachinesHandler.main.GetMachine(quest.machineQuest.machineNeededName);
                progress.text = machine.qnt + "/" + quest.machineQuest.neededAmount;

                break;

            case QuestType.ReachMoney:
                progress.text = ResourcesHandler.money + "/" + quest.moneyQuest.moneyAmount;

                break;
        }

    }


}
