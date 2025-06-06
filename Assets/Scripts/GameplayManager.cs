using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    [Header("TRANSIÇÃO")]
    [SerializeField] private GameObject blackoutPanel;

    [Header("MALETA")]
    [SerializeField] private GameObject briefcasePF;
    [SerializeField] private List<Transform> briefcaseSpawnLocations;
    [SerializeField] private Transform briefcaseCanvas;

    private void Awake()
    {
        blackoutPanel.SetActive(true);
    }

    private void Start()
    {
        StartCoroutine(DropBriefcase());
        blackoutPanel.GetComponent<UITweener>().FadeAlpha(0f, 2f, true);
        AudioManager.StartMusic(AudioManager.main.ambience, .3f);
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
            GameObject briefcase = 
            Instantiate(briefcasePF, briefcaseSpawnLocations[Random.Range(0, briefcaseSpawnLocations.Count)].position, Quaternion.identity, briefcaseCanvas);

            yield return new WaitForSeconds(4f);

            Destroy(briefcase);
        }

        StartCoroutine(DropBriefcase());
    }
}
