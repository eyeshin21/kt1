using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelInfoUI : MonoBehaviour
{
    [SerializeField] RectTransform rectTransform;
    [SerializeField] TextMeshProUGUI levelTxt;
    [SerializeField] RectTransform levelRect;
    [SerializeField] RectTransform lineRect;
    [SerializeField] GameObject fillImg;
    [SerializeField] ParticleSystem vfx;
    [SerializeField] Color[] colorVFX;
    [SerializeField] Image boardImg;
    [SerializeField] Sprite[] boardSprs;

    public void Init(int level)
    {
        if (level > 0)
        {
            //if (level % 2 == 0)
            //{
            //    rectTransform.anchoredPosition = new Vector2(119f, rectTransform.anchoredPosition.y);
            //    lineRect.anchoredPosition = new Vector2(18f, lineRect.anchoredPosition.y);
            //    lineRect.localScale = new Vector3(-1, 1, 1);
            //}
            //else
            //{
            //    rectTransform.anchoredPosition = new Vector2(-129f, rectTransform.anchoredPosition.y);
            //    lineRect.anchoredPosition = new Vector2(-18f, lineRect.anchoredPosition.y);
            //    lineRect.localScale = Vector3.one;
            //}

            gameObject.SetActive(true);
            eTypeLevel eTypeLevel = GameManager.Instance.GetTypeLevel(level);

            //levelRect.anchoredPosition = new Vector2(0, 20);

            levelTxt.text = level.ToString();
            switch (eTypeLevel)
            {
                case eTypeLevel.Normal:
                case eTypeLevel.Tutorial:
                    if (level < UserConfig.Instance.CurLevel)
                    {
                        boardImg.sprite = boardSprs[1];
                        vfx.gameObject.SetActive(false);
                        boardImg.SetNativeSize();
                        boardImg.transform.localScale = Vector3.one;
                    }
                    else if (level == UserConfig.Instance.CurLevel)
                    {
                        boardImg.sprite = boardSprs[2];
                        vfx.startColor = colorVFX[0];
                        vfx.gameObject.SetActive(true);
                        boardImg.SetNativeSize();
                        boardImg.transform.localScale = Vector3.one;
                    }
                    else
                    {
                        boardImg.sprite = boardSprs[0];
                        vfx.gameObject.SetActive(false);
                        boardImg.SetNativeSize();
                        boardImg.transform.localScale = Vector3.one;

                        //levelRect.anchoredPosition = new Vector2(0, 4);
                    }
                    break;
                case eTypeLevel.Hard:
                    //levelRect.anchoredPosition = new Vector2(0, -18);
                    if (level < UserConfig.Instance.CurLevel)
                    {
                        boardImg.sprite = boardSprs[3];
                        vfx.gameObject.SetActive(false);
                        boardImg.SetNativeSize();
                        boardImg.transform.localScale = Vector3.one;
                    }
                    else if (level == UserConfig.Instance.CurLevel)
                    {
                        boardImg.sprite = boardSprs[3];
                        vfx.startColor = colorVFX[1];
                        vfx.gameObject.SetActive(true);
                        boardImg.SetNativeSize();
                        boardImg.transform.localScale = Vector3.one;
                    }
                    else
                    {
                        boardImg.sprite = boardSprs[3];
                        vfx.gameObject.SetActive(false);
                        boardImg.SetNativeSize();
                        boardImg.transform.localScale = new Vector3(0.9f, 0.9f, 1);
                    }

                    break;
                case eTypeLevel.SuperHard:
                    //levelRect.anchoredPosition = new Vector2(0, -18);
                    if (level < UserConfig.Instance.CurLevel)
                    {
                        boardImg.sprite = boardSprs[4];
                        vfx.gameObject.SetActive(false);
                        boardImg.SetNativeSize();
                        boardImg.transform.localScale = Vector3.one;
                    }
                    else if (level == UserConfig.Instance.CurLevel)
                    {
                        boardImg.sprite = boardSprs[4];
                        vfx.startColor = colorVFX[2];
                        vfx.gameObject.SetActive(true);
                        boardImg.SetNativeSize();
                        boardImg.transform.localScale = Vector3.one;
                    }
                    else
                    {
                        boardImg.sprite = boardSprs[4];
                        vfx.gameObject.SetActive(false);
                        boardImg.SetNativeSize();
                        boardImg.transform.localScale = new Vector3(0.9f, 0.9f, 1);
                    }

                    break;
            }

            if (level <= UserConfig.Instance.CurLevel - 1)
            {
                fillImg.SetActive(true);
            }
            else
            {
                fillImg.SetActive(false);
            }
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
