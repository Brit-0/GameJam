using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour
{
    private Animator animator;
    private GameObject logo, blackoutPanel;
    private bool canStart;  

    private void Awake()
    {
        animator = GetComponent<Animator>();
        logo = transform.Find("Logo").gameObject;     
        blackoutPanel = transform.Find("BlackoutPanel").gameObject;
    }

    private void Start()
    {
        AudioManager.StartMusic(AudioManager.main.ambience);
        animator.SetTrigger("Loaded");
    }

    public void CallLogoFade()
    {
        logo.GetComponent<UITweener>().FadeAlpha(1f, 1.8f);
    }

    public void DefineCanStart()
    {
        canStart = true;
    }

    public void StartInitiateCoroutine()
    {
        if (!canStart) return;

        StartCoroutine(StartGameTransition());
    }

    private IEnumerator StartGameTransition()
    {
        blackoutPanel.GetComponent<UITweener>().FadeAlpha(1f, 2f);

        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene("Gameplay");
    }
}
