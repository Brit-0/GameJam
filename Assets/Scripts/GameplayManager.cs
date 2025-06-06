using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    [SerializeField] private GameObject blackoutPanel;

    private void Awake()
    {
        blackoutPanel.SetActive(true);
    }

    private void Start()
    {
        blackoutPanel.GetComponent<UITweener>().FadeAlpha(0f, 2f);
    }
}
