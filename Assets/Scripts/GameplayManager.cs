using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        AudioManager.StartMusic(AudioManager.main.music, .3f);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            AudioManager.PlayOverlayAudio(AudioManager.main.click, .7f);
        }

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.L) && Input.GetKeyDown(KeyCode.M))
        {
            ResourcesHandler.money += 100;
        }

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("Menu");
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
