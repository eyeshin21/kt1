using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CGTeam
{
    public class MainMenu : UIMenu
    {
        [SerializeField] LevelInfoUI[] levelInfoUIs;

        [HideInInspector] public GameObject objBtnRemoveAds;
        [SerializeField] public GameObject objBtnStarterPack;

        [SerializeField] GameObject objBtnPlayNormal;
        [SerializeField] GameObject objBtnPlayHard;
        [SerializeField] GameObject objBtnPlaySuperHard;

        public override void Start()
        {
            base.Start();

            CheckNonConsumable();
        }

        public override void Show()
        {
            base.Show();

            EconomyMenu.instance.Show();

            for (int i = 0; i < levelInfoUIs.Length; i++)
            {
                levelInfoUIs[i].Init(UserConfig.Instance.CurLevel - 4 + i);
            }

            eTypeLevel eTypeLevel = GameManager.Instance.GetTypeLevel(UserConfig.Instance.CurLevel);

            objBtnPlayNormal.SetActive(eTypeLevel == eTypeLevel.Normal || eTypeLevel == eTypeLevel.Tutorial);
            objBtnPlayHard.SetActive(eTypeLevel == eTypeLevel.Hard);
            objBtnPlaySuperHard.SetActive(eTypeLevel == eTypeLevel.SuperHard);
        }

        public void PressedPlayBtn()
        {
            UnityEvent e = new UnityEvent();
            e.AddListener(() =>
            {
                Hide();
                GameManager.Instance.LoadLevel();
                UIManager.Instance.ShowIngameMenu();
            });
            FadeMenu.Instance.Fade(e, true);
        }

        public void PressedSettingBtn()
        {
            UIManager.Instance.ShowSettingMenu();
        }

        public void PressedShopBtn()
        {
            UIManager.Instance.ShowShopMenu();
        }

        public void PressedRemoveAdsBtn()
        {
            UIManager.Instance.ShowRemoveAdsMenu();
        }

        public void PressedStarterPackBtn()
        {
            UIManager.Instance.ShowStarterPackMenu();
        }

        public void CheckNonConsumable()
        {
            //if (CGTeamBridge.Instance.IsNonConsumablePurchased(eIAPKey.kRemoveAdsBundle))
            //{
            //    objBtnRemoveAds.SetActive(false);
            //}
            //else
            //{
            //    objBtnRemoveAds.SetActive(true);
            //}

            if (Bridge.Instance.IsNonConsumablePurchased(eIAPKey.kStarterPack))
            {
                objBtnStarterPack.SetActive(false);
            }
            else
            {
                objBtnStarterPack.SetActive(true);
            }
        }

        public override void Hide()
        {
            base.Hide();
            //UIManager.Instance.tabMenu.Hide();
            gameObject.SetActive(false);
        }
    }
}