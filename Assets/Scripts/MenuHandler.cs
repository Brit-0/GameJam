using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuHandler : MonoBehaviour
{
    private Animator animator;
    private GameObject logo, startLbl, blackoutPanel;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        logo = transform.Find("Logo").gameObject;
        startLbl = transform.Find("StartLbl").gameObject;
        blackoutPanel = transform.Find("BlackoutPanel").gameObject;
    }

    private void Start()
    {
        animator.SetTrigger("Loaded");
    }

    public void CallLogoFade()
    {
        logo.GetComponent<UITweener>().FadeAlpha(1f, 1.8f);
    }

    public void StartInitiateCoroutine()
    {
        StartCoroutine(StartGameTransition());
    }

    private IEnumerator StartGameTransition()
    {
        blackoutPanel.GetComponent<UITweener>().FadeAlpha(1f, 2f);

        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene("Gameplay");
    }
}
