using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BriefcaseLogic : MonoBehaviour
{
    private bool alreadyRewarded;
    private TextMeshProUGUI rewardLbl;

    [SerializeField] bool isRare;

    private void Awake()
    {
        rewardLbl = transform.Find("RewardLbl").GetComponent<TextMeshProUGUI>();
    }

    public void BriefcaseReward()
    {
        if (alreadyRewarded) return;
        alreadyRewarded = true;

        int moneyReward;

        if (isRare)
        {
            moneyReward = Random.Range(500, 1001);
        }
        else
        {
            moneyReward = Random.Range(20, 101);
        }

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
