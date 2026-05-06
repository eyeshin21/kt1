using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.Events;
using TigerForge;

public class EconomyMenu : MonoBehaviour
{
    [SerializeField] GameObject content;
    [SerializeField] GameObject blockUI;

    [Header("Coin")]
    [SerializeField] TextMeshProUGUI coinText;
    [SerializeField] Transform coinImg;
    [SerializeField] GameObject coinUIPrefab;
    [SerializeField] ParticleSystem coin_vfx;

    public static EconomyMenu instance;

    [HideInInspector]
    public bool isShowing = true;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        isShowing = true;
    }

    private void Start()
    {
        SetCoinTxt(UserConfig.Instance.Coin);

        blockUI.SetActive(false);
    }

    public void Hide()
    {
        isShowing = false;
        content.SetActive(false);
    }

    public void Show()
    {
        isShowing = true;
        content.SetActive(true);
    }

    #region Coin
    public void PressedCoinBtn()
    {
        if (UIManager.Instance.shopMenu == null || !UIManager.Instance.shopMenu.gameObject.activeInHierarchy)
        {
            if (UIManager.Instance.ingameMenu.gameObject.activeSelf)
            {
                if (ShooterManager.Instance.shooters.Count > 0)
                {
                    UIManager.Instance.ShowShopMenu();
                }
            }
            else
            {
                UIManager.Instance.ShowShopMenu();
            }
        }
    }

    public void AddGold(int coinAdd, Vector3 posWorld)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(posWorld);

        float minX = screenPos.x - 50f;
        float maxX = screenPos.x + 50f;
        float minY = screenPos.y - 50f;
        float maxY = screenPos.y + 50f;
        for (int i = 0; i < 10; i++)
        {
            GameObject obj = coinUIPrefab.Spawn(transform);
            obj.transform.position = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), transform.position.z);
            obj.transform.localScale = Vector3.zero;
            obj.name = (i + 1).ToString();
            int temp = i;
            obj.transform.DOScale(Vector3.one, 0.2f).SetDelay(0.05f * i).SetEase(Ease.OutBack).OnComplete(() =>
            {
                if (temp == 0)
                {
                    obj.transform.DOMove(coinImg.position, 0.25f).SetEase(Ease.Linear).SetDelay(0.15f).OnComplete(() =>
                    {
                        SoundManager.instance.PlaySound("Coin");
                        StartCoroutine(EffectCoinMoneyPanel(coinAdd));
                        obj.Recycle();
                    });
                }
                else if (temp == 9)
                {
                    obj.transform.DOMove(coinImg.position, 0.25f).SetEase(Ease.Linear).SetDelay(0.15f).OnComplete(() =>
                    {
                        SoundManager.instance.PlaySound("Coin");
                        obj.Recycle();
                    });
                }
                else
                {
                    obj.transform.DOMove(coinImg.position, 0.25f).SetEase(Ease.Linear).SetDelay(0.15f).OnComplete(() =>
                    {
                        SoundManager.instance.PlaySound("Coin");
                        obj.Recycle();
                    });
                }
            });
        }
    }

    public void AddGold(int coinAdd, Transform rectUI, UnityEvent callback)
    {
        if (rectUI != null)
        {
            float minX = rectUI.position.x - 100.75f;
            float maxX = rectUI.position.x + 100.75f;
            float minY = rectUI.position.y - 110.0f;
            float maxY = rectUI.position.y + 110.0f;
            for (int i = 0; i < 10; i++)
            {
                GameObject obj = coinUIPrefab.Spawn(transform);
                obj.transform.position = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), transform.position.z);
                obj.transform.localScale = Vector3.zero;
                obj.name = (i + 1).ToString();
                int temp = i;
                obj.transform.DOScale(Vector3.one, 0.2f).SetDelay(0.05f * i).SetEase(Ease.OutBack).OnComplete(() =>
                {
                    if (temp == 0)
                    {
                        obj.transform.DOMove(coinImg.position, 0.25f).SetEase(Ease.Linear).SetDelay(0.15f).OnComplete(() =>
                        {
                            SoundManager.instance.PlaySound("Coin");
                            StartCoroutine(EffectCoinMoneyPanel(coinAdd));
                            obj.Recycle();
                        });
                    }
                    else if (temp == 9)
                    {
                        obj.transform.DOMove(coinImg.position, 0.25f).SetEase(Ease.Linear).SetDelay(0.15f).OnComplete(() =>
                        {
                            SoundManager.instance.PlaySound("Coin");
                            obj.Recycle();
                            if (callback != null)
                            {
                                callback.Invoke();
                            }
                        });
                    }
                    else
                    {
                        obj.transform.DOMove(coinImg.position, 0.25f).SetEase(Ease.Linear).SetDelay(0.15f).OnComplete(() =>
                        {
                            SoundManager.instance.PlaySound("Coin");
                            obj.Recycle();
                        });
                    }
                });
            }
        }
        else
        {
            if (gameObject.activeSelf)
            {
                StartCoroutine(EffectCoinMoneyPanel(coinAdd));
            }
            else
            {
                UserConfig.Instance.Coin += coinAdd;
            }
        }
    }

    public IEnumerator EffectCoinMoneyPanel(int coinAdd)
    {
        WaitForSeconds delay = new WaitForSeconds(0.02f);
        if (coinAdd > 0)
        {
            UserConfig.Instance.TotalSpend += coinAdd;
            coin_vfx.Play();
        }
        else
        {
            UserConfig.Instance.TotalEarn += coinAdd;
        }

        var coin = UserConfig.Instance.Coin;
        UserConfig.Instance.Coin += coinAdd;

        if (Mathf.Abs(coinAdd) < 10)
        {
            for (var i = 0; i < Mathf.Abs(coinAdd); i++)
            {
                yield return delay;
                coin += (int)Mathf.Sign(coinAdd);
                SetCoinTxt(coin);
            }
        }
        else
        {
            for (var i = 0; i < 10; i++)
            {
                yield return delay;
                if (i == 9)
                {
                    coin += coinAdd - (coinAdd / 10) * 9;
                }
                else
                {
                    coin += coinAdd / 10;
                }
                SetCoinTxt(coin);
            }
        }
    }

    public void SetCoinTxt(int coin)
    {
        coinText.text = coin.ToString();
    }

    public void PlayVFXCoin()
    {
        coin_vfx.Play();
    }
    #endregion
}
