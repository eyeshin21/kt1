using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AdsLoadingMenu : UIMenu
{
    [SerializeField] TextMeshProUGUI txt;
    public string content;

    public override void Show()
    {
        base.Show();

        txt.text = content;
    }

    public override void Hide()
    {
        base.Hide();
        gameObject.SetActive(false);
    }
}
