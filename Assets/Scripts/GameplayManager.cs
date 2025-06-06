using System.Collections;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    [Header("TRANSIÇÃO")]
    [SerializeField] private GameObject blackoutPanel;

    [Header("MALETA")]
    [SerializeField] private GameObject briefcasePF;
    [SerializeField] private Transform briefcaseSpawnLocation;
    [SerializeField] private GameObject overlayCanvas;

    private void Awake()
    {
        blackoutPanel.SetActive(true);
    }

    private void Start()
    {
        StartCoroutine(DropBriefcase());
        blackoutPanel.GetComponent<UITweener>().FadeAlpha(0f, 2f, true);
        AudioManager.StartMusic(AudioManager.main.music, .3f);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            AudioManager.PlayOverlayAudio(AudioManager.main.click, .7f);
        }
    }

    private IEnumerator DropBriefcase()
    {
        yield return new WaitForSeconds(30f);

        if (Random.Range(0, 2) == 0)
        {
            GameObject briefcase = Instantiate(briefcasePF, briefcaseSpawnLocation.position, Quaternion.identity);

            yield return new WaitForSeconds(5f);

            Destroy(briefcase);
        }
    }

    public void BriefcaseReward()
    {

    }
}
