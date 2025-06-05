using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestBar : MonoBehaviour
{  
    private TextMeshProUGUI description, progress;
    private QuestData quest;
    private bool wasColorSet;

    private void Awake()
    {
        description = transform.Find("Description").GetComponent<TextMeshProUGUI>();
        progress = transform.Find("Progress").GetComponent<TextMeshProUGUI>();
    }

    public void SetBar(QuestData quest)
    {
        this.quest = quest;

        description.text = quest.description;
        progress.text = ResourcesHandler.main.GetResource(quest.resourceQuest.resourceNeeded) + " / " + quest.resourceQuest.neededAmount;
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
            progress.text = ResourcesHandler.main.GetResource(quest.resourceQuest.resourceNeeded) + " / " + quest.resourceQuest.neededAmount;
        }
    }


}
