using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BriefcaseLogic : MonoBehaviour
{
    private bool alreadyRewarded;
    private TextMeshProUGUI rewardLbl;

    private void Awake()
    {
        rewardLbl = transform.Find("RewardLbl").GetComponent<TextMeshProUGUI>();
    }

    public void BriefcaseReward()
    {
        if (alreadyRewarded) return;
        alreadyRewarded = true;

        int moneyReward = Random.Range(20, 101);

        rewardLbl.text = "+" + moneyReward;
        rewardLbl.gameObject.SetActive(true);

        ResourcesHandler.money += moneyReward;

        StartCoroutine(DestroyBriefcase());
    }

    private IEnumerator DestroyBriefcase()
    {
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition;

        yield return new WaitForSeconds(1f);

        Destroy(gameObject);
    }
}
