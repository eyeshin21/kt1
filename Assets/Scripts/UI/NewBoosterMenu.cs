using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum eTypeBooster
{
    Undo = 0,
    ExtraSlot = 1,
    Swap = 2,
    Magnet = 3
}

public class NewBoosterMenu : UIMenu
{
    [HideInInspector] public eTypeBooster eTypeBooster;

    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] CanvasGroup canvasGroup2;
    [SerializeField] Image imgBg;
    [SerializeField] TextMeshProUGUI nameTxt;
    [SerializeField] TextMeshProUGUI contentTxt;
    [SerializeField] RectTransform rtsfIcon;
    [SerializeField] Image iconImg;
    [SerializeField] AnimatedButton btnClaim;

    [Header("VFX")]
    [SerializeField] RectTransform rtsfVfxAddAmount;
    [SerializeField] ParticleSystem vfxAddAmount;

    [Header("Tutorial")]
    [SerializeField] RectTransform handRect;
    [SerializeField] Animator animHand;
    [SerializeField] GameObject labelTut;
    [SerializeField] TextMeshProUGUI txtTut;

    [Header("Booster")]
    [SerializeField] GameObject boosterUndo;
    [SerializeField] GameObject boosterExtraSlot;
    [SerializeField] GameObject boosterSwap;
    [SerializeField] GameObject boosterMagnet;
    
    [SerializeField] TextMeshProUGUI txtAmountBoosterUndo;
    [SerializeField] TextMeshProUGUI txtAmountBoosterExtraSlot;
    [SerializeField] TextMeshProUGUI txtAmountBoosterSwap;
    [SerializeField] TextMeshProUGUI txtAmountBoosterMagnet;

    public override void Show()
    {
        base.Show();

        boosterUndo.SetActive(false);
        boosterExtraSlot.SetActive(false);
        boosterSwap.SetActive(false);
        boosterMagnet.SetActive(false);
        animHand.gameObject.SetActive(false);
        labelTut.SetActive(false);

        switch (eTypeBooster)
        {
            case eTypeBooster.Undo:
                nameTxt.text = "Undo";
                contentTxt.text = "Move the placed hook back to its original position.";
                txtTut.text = "";
                break;
            case eTypeBooster.ExtraSlot:
                nameTxt.text = "ExtraSlot";
                contentTxt.text = "Move the three queued hooks up one row.";
                txtTut.text = "";
                break;
            case eTypeBooster.Swap:
                nameTxt.text = "Swap";
                contentTxt.text = "Swap one hook in the queue with any random hook.";
                txtTut.text = "";
                break;
            case eTypeBooster.Magnet:
                nameTxt.text = "Magnet";
                contentTxt.text = "Instantly fill a selected waiting hook.";
                txtTut.text = "";
                break;
        }

        iconImg.sprite = GameManager.Instance.LoadSprite("Icons/Boosters/" + eTypeBooster);
        iconImg.SetNativeSize();

        imgBg.color = new Color(0, 0, 0, 0.9411765f);

        canvasGroup2.alpha = 1;
        rtsfIcon.gameObject.SetActive(true);
        rtsfIcon.anchoredPosition = new Vector2(0, 197);
        rtsfIcon.sizeDelta = new Vector2(250, 250);

        btnClaim.interactable = true;

        canvasGroup.alpha = 0;
        canvasGroup.DOFade(1f, 0.25f).SetEase(Ease.Linear);
    }

    public override void Hide()
    {
        base.Hide();
        gameObject.SetActive(false);
    }

    public void OnClickButtonClaim()
    {
        btnClaim.interactable = false;

        Vector2 target = Vector2.zero;
        switch (eTypeBooster)
        {
            case eTypeBooster.Undo:
                target = new Vector2(-339f, 132f - GameManager.Instance.screenHeight / 2.0f + GameManager.Instance.safeAreaYBottom);
                break;
            case eTypeBooster.ExtraSlot:
                target = new Vector2(-113f, 132f - GameManager.Instance.screenHeight / 2.0f + GameManager.Instance.safeAreaYBottom);
                break;
            case eTypeBooster.Swap:
                target = new Vector2(113f, 132f - GameManager.Instance.screenHeight / 2.0f + GameManager.Instance.safeAreaYBottom);
                break;
            case eTypeBooster.Magnet:
                target = new Vector2(339f, 132f - GameManager.Instance.screenHeight / 2.0f + GameManager.Instance.safeAreaYBottom);
                break;
        }

        imgBg.DOFade(0.6862745f, 0.5f);
        canvasGroup2.DOFade(0f, 0.5f).SetEase(Ease.Linear);
        rtsfIcon.DOSizeDelta(new Vector2(110, 110), 0.5f).SetEase(Ease.Linear);
        rtsfIcon.DOAnchorPos(target, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
        {
            //if (eTypeBooster == eTypeBooster.Clear)
            //{
            //    Hide();
            //    return;
            //}

            //labelTut.SetActive(true);

            rtsfIcon.gameObject.SetActive(false);
            handRect.gameObject.SetActive(true);
            handRect.anchoredPosition = target;
            animHand.Play("Base Layer.TutorialTap", 0, 0);

            rtsfVfxAddAmount.anchoredPosition = target;
            vfxAddAmount.Play();

            switch (eTypeBooster)
            {
                case eTypeBooster.Undo:
                    boosterUndo.SetActive(true);
                    txtAmountBoosterUndo.text = UserConfig.Instance.AmountUndo.ToString();
                    break;
                case eTypeBooster.ExtraSlot:
                    boosterExtraSlot.SetActive(true);
                    txtAmountBoosterExtraSlot.text = UserConfig.Instance.AmountExtraSlot.ToString();
                    break;
                case eTypeBooster.Swap:
                    boosterSwap.SetActive(true);
                    txtAmountBoosterSwap.text = UserConfig.Instance.AmountSwap.ToString();
                    break;
                case eTypeBooster.Magnet:
                    boosterMagnet.SetActive(true);
                    txtAmountBoosterMagnet.text = UserConfig.Instance.AmountMagnet.ToString();
                    break;
            }
        });
    }

    public void OnClickButtonUndo()
    {
        Hide();
        UIManager.Instance.ingameMenu.PressedItemUndoBtn();
    }    
    
    public void OnClickButtonExtraSlot()
    {
        Hide();
        UIManager.Instance.ingameMenu.PressedItemExtraSlotBtn();
    }

    public void OnClickButtonSwap()
    {
        Hide();
        UIManager.Instance.ShowItemUsingMenu(eBooster.Swap, true);
    }
    public void OnClickButtonMagnet()
    {
        Hide();
        UIManager.Instance.ShowItemUsingMenu(eBooster.Magnet, true);
    }
}
