using UnityEngine;

public class UITweener : MonoBehaviour
{
    RectTransform rectTrans;

    //public UnityEvent closeTab; //DEFINIR NO INSPECTOR

    private void Awake()
    {
        rectTrans = GetComponent<RectTransform>();
    }

    public void CloseTween()
    {
        var seq = LeanTween.sequence();
        seq.append(LeanTween.scale(rectTrans, Vector2.zero, .2f));
        seq.append(() => rectTrans.gameObject.SetActive(false));
    }

    public void PopUpTween(float scale)
    {
        rectTrans.localScale = Vector3.zero;
        LeanTween.scale(rectTrans, new Vector3(scale, scale, scale), .2f);
    }

    public void ClickTween(float clickTime)
    {
        if (!LeanTween.isTweening(rectTrans))
        {
            LeanTween.scale(rectTrans, rectTrans.localScale * 0.8f, clickTime).setLoopPingPong(1);
        }
    }

    public void AlphaFlick()
    {
        if (LeanTween.isTweening(rectTrans)) return;

        LeanTween.alpha(rectTrans, .8f, 2f).setLoopPingPong(1);
    }

    public void FadeOut()
    {
        var seq = LeanTween.sequence();
        seq.append(LeanTween.alpha(rectTrans, 0, .5f));
        seq.append(() => { rectTrans.gameObject.SetActive(false); } );
        seq.append(LeanTween.alpha(rectTrans, 1, .5f));
    }

    public void ColorFlickTween(Color color, float speed)
    {
        LeanTween.color(rectTrans, color, speed).setLoopPingPong(1);
    }

    public void FadeAlpha(float toAlpha, float time, bool fadeOut = default(bool))
    {
        var seq = LeanTween.sequence();
        seq.append(LeanTween.alpha(rectTrans, toAlpha, time));
        seq.append(() => 
        {
            if (fadeOut)
            {
                rectTrans.gameObject.SetActive(false);
            }
        });
    }
}
