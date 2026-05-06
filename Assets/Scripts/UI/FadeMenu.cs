using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;

public class FadeMenu : Singleton<FadeMenu>
{
    [SerializeField] CanvasGroup fadeMenu;
    [SerializeField] Image progressImg;

    UnityEvent unityEvent;

    [HideInInspector] public bool fadeOut = false;

    public static bool IsLowMemoryDevice()
    {
        return !SupportASTCFormat() && SystemInfo.graphicsMemorySize < 512;
    }

    public static bool SupportASTCFormat()
    {
        return SystemInfo.SupportsTextureFormat(TextureFormat.ASTC_4x4);
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        progressImg.fillAmount = 1;

        yield return new WaitForSeconds(0.5f);

        if (UserConfig.Instance.CurLevel < 2)
        {
            //yield return new WaitUntil(() => GameManager.Instance.currentLevel != null);
            //yield return new WaitUntil(() => GameManager.Instance.currentLevel.doneLoadLevel);

            yield return new WaitUntil(() => GameplayController.Instance.doneLoadLevel);
        }

        fadeMenu.DOFade(0, 0.1f).SetEase(Ease.Linear).OnStart(() =>
        {
            fadeOut = true;
        }).OnComplete(() =>
        {
            fadeMenu.gameObject.SetActive(false);
            fadeOut = false;
        });
    }

    void FadeIn(UnityEvent unityEvent)
    {
        fadeMenu.gameObject.SetActive(true);
        fadeMenu.alpha = 0;
        progressImg.fillAmount = 0;
        this.unityEvent = unityEvent;

        fadeMenu.DOFade(1, 0.2f).SetEase(Ease.Linear);
    }

    void FadeOut()
    {
        // Grab a free Sequence to use
        Sequence mySequence = DOTween.Sequence();
        // Add a movement tween at the beginning
        mySequence.Append(progressImg.DOFillAmount(1, 1.0f).SetEase(Ease.Linear));
        mySequence.Append(fadeMenu.DOFade(0, 0.1f).SetEase(Ease.Linear).OnStart(() =>
        {
            fadeOut = true;
        }).OnComplete(() =>
        {
            fadeMenu.gameObject.SetActive(false);
            fadeOut = false;
        }));
    }

    public void Fade(UnityEvent unityEvent, bool waitForLoadLevel = false)
    {
        StartCoroutine(CoroutineFade(unityEvent, waitForLoadLevel));
    }

    IEnumerator CoroutineFade(UnityEvent unityEvent, bool waitForLoadLevel)
    {
        FadeIn(unityEvent);
        yield return new WaitForSeconds(0.3f);
        unityEvent?.Invoke();
        yield return new WaitForSeconds(0.5f);

        if (waitForLoadLevel)
        {
            yield return new WaitForEndOfFrame();
            //yield return new WaitUntil(() => GameManager.Instance.currentLevel != null);
            //yield return new WaitUntil(() => GameManager.Instance.currentLevel.doneLoadLevel);
            yield return new WaitUntil(() => GameplayController.Instance.doneLoadLevel);
        }

        FadeOut();
    }
}