using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AwardItemUi : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI amountTxt;

    public void Init(eSystemItemType eSystemItemType, int amount)
    {
        icon.sprite = GameManager.Instance.LoadSprite("Icons/SystemItem/" + eSystemItemType.ToString());
        icon.SetNativeSize();

        amountTxt.text = "x" + amount;
    }
}
