using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class GameplayManager : MonoBehaviour
{
    [Header("MENU")]
    [SerializeField] private GameObject quitPanel;

    [Header("TRANSIÇÃO")]
    [SerializeField] private GameObject blackoutPanel;

    [Header("MALETA")]
    [SerializeField] private GameObject briefcasePF;
    [SerializeField] private GameObject rareBriefcasePF;
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
            ResourcesHandler.money += 500;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (quitPanel.activeInHierarchy)
            {
                quitPanel.GetComponent<UITweener>().CloseTween();
            }
            else
            {
                quitPanel.SetActive(true);
                quitPanel.GetComponent<UITweener>().PopUpTween(1f);
            }
        }
    }

    public void ResetGame()
    {
        SceneManager.LoadScene("Menu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private IEnumerator DropBriefcase()
    {
        yield return new WaitForSeconds(30f);

        GameObject pf;

        if (Random.Range(1, 11) == 10)
        {
            pf = rareBriefcasePF;
        }
        else
        {
            pf = briefcasePF;
        }

        if (Random.Range(0, 2) == 0)
        {
            GameObject briefcase =
            Instantiate(pf, briefcaseSpawnLocations[Random.Range(0, briefcaseSpawnLocations.Count)].position, Quaternion.identity, briefcaseCanvas);

            yield return new WaitForSeconds(4f);

            Destroy(briefcase);
        }

        StartCoroutine(DropBriefcase());
    }
}
