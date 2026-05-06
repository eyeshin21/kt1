using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserConfig
{
    private static UserConfig _instance;

    public static UserConfig Instance
    {
        get
        {
            if (UserConfig._instance == null)
            {
                UserConfig._instance = new UserConfig();
            }
            return UserConfig._instance;
        }
    }

    public int RetendDay
    {
        get
        {
            return PlayerPrefs.GetInt(GameplayVariables.retendDay, 0);
        }
        set
        {
            PlayerPrefs.SetInt(GameplayVariables.retendDay, value);
        }
    }

    public int DayPlayed
    {
        get
        {
            return PlayerPrefs.GetInt(GameplayVariables.dayPlayed, 0);
        }
        set
        {
            PlayerPrefs.SetInt(GameplayVariables.dayPlayed, value);
        }
    }

    public bool Sound
    {
        get
        {
            return sound;
        }
        set
        {
            sound = value;
            PlayerPrefs.SetInt(GameplayVariables.sound, value ? 1 : 0);
        }
    }

    public bool Music
    {
        get
        {
            return music;
        }
        set
        {
            music = value;
            PlayerPrefs.SetInt(GameplayVariables.music, value ? 1 : 0);
        }
    }

    public bool Vibrate
    {
        get
        {
            return vibrate;
        }
        set
        {
            vibrate = value;
            PlayerPrefs.SetInt(GameplayVariables.vibrate, value ? 1 : 0);
        }
    }

    public int BestLevel
    {
        get
        {
            return bestLevel;
        }
        set
        {
            bestLevel = value;
            //CGTeamBridge.instance.SetPropertyLevel((bestLevel - 1).ToString());
            PlayerPrefs.SetInt(GameplayVariables.bestLevel, value);
        }
    }

    public int CurLevel
    {
        get
        {
            return curLevel;
        }
        set
        {
            curLevel = value;
            if (curLevel > BestLevel)
            {
                BestLevel = value;
            }
        }
    }

    public int Coin
    {
        get
        {
            return coin;
        }
        set
        {
            if (value < 0)
            {
                value = 0;
            }
            coin = value;
            PlayerPrefs.SetInt(GameplayVariables.coin, value);
        }
    }

    public int Ticket
    {
        get
        {
            return ticket;
        }
        set
        {
            if (value < 0)
            {
                value = 0;
            }
            ticket = value;
            PlayerPrefs.SetInt(GameplayVariables.ticket, value);
        }
    }

    public int Live
    {
        get
        {
            return live;
        }
        set
        {
            //live = Mathf.Clamp(value, 0, GameManager.MAX_LIVE);
            PlayerPrefs.SetInt(GameplayVariables.live, live);
        }
    }

    public bool Rate
    {
        get
        {
            return rate;
        }
        set
        {
            rate = value;
            PlayerPrefs.SetInt(GameplayVariables.rate, value ? 1 : 0);
        }
    }

    public bool HasAds
    {
        get
        {
            return hasAds;
        }
        set
        {
            hasAds = value;
            PlayerPrefs.SetInt(GameplayVariables.hasAds, value ? 1 : 0);
        }
    }

    public bool HasInfinityLive
    {
        get
        {
            return hasInfinityLive;
        }
        set
        {
            hasInfinityLive = value;
            PlayerPrefs.SetInt(GameplayVariables.hasInfinityLive, value ? 1 : 0);
        }
    }

    public int DailyLogin
    {
        get
        {
            return dailyLogin;
        }
        set
        {
            dailyLogin = value;
            PlayerPrefs.SetInt(GameplayVariables.dailyLogin, value);
        }
    }

    public int TotalSpend
    {
        get
        {
            return totalSpend;
        }
        set
        {
            totalSpend = value;
            //CGTeamBridge.instance.SetPropertyCoinEarn(totalSpend.ToString());
            PlayerPrefs.SetInt(GameplayVariables.totalSpend, value);
        }
    }

    public int TotalEarn
    {
        get
        {
            return totalEarn;
        }
        set
        {
            totalEarn = value;
            //CGTeamBridge.instance.SetPropertyCoinEarn(totalEarn.ToString());
            PlayerPrefs.SetInt(GameplayVariables.totalEarn, value);
        }
    }

    public int WinLevel
    {
        get
        {
            return winLevel;
        }
        set
        {
            winLevel = value;
            PlayerPrefs.SetInt(GameplayVariables.winLevel, value);
        }
    }

    public float LoseProgress
    {
        get
        {
            return loseProgress;
        }
        set
        {
            loseProgress = value;
            PlayerPrefs.SetFloat(GameplayVariables.loseProgress, value);
        }
    }

    public int AmountInter
    {
        get
        {
            return amountInter;
        }
        set
        {
            amountInter = value;
            PlayerPrefs.SetInt(GameplayVariables.amountInter, value);
        }
    }

    public int AmountRw
    {
        get
        {
            return amountRw;
        }
        set
        {
            amountRw = value;
            PlayerPrefs.SetInt(GameplayVariables.amountRw, value);
        }
    }

    public int AmountUndo
    {
        get
        {
            return amountUndo;
        }
        set
        {
            amountUndo = value;
            PlayerPrefs.SetInt(GameplayVariables.amountUndo, value);
        }
    }

    public int AmountExtraSlot
    {
        get
        {
            return amountExtraSlot;
        }
        set
        {
            amountExtraSlot = value;
            PlayerPrefs.SetInt(GameplayVariables.amountExtraSlot, value);
        }
    }

    public int AmountSwap
    {
        get
        {
            return amountSwap;
        }
        set
        {
            amountSwap = value;
            PlayerPrefs.SetInt(GameplayVariables.amountSwap, value);
        }
    }  

    public int AmountMagnet
    {
        get
        {
            return amountMagnet;
        }
        set
        {
            amountMagnet = value;
            PlayerPrefs.SetInt(GameplayVariables.amountMagnet, value);
        }
    }  
    
    public int LevelShowRemoveAds
    {
        get
        {
            return levelShowRemoveAds;
        }
        set
        {
            levelShowRemoveAds = value;
            PlayerPrefs.SetInt(GameplayVariables.levelShowRemoveAds, value);
        }
    }

    public float StartTimeResetEnergy
    {
        get
        {
            return startTimeResetEnergy;
        }
        set
        {
            startTimeResetEnergy = value;
            PlayerPrefs.SetFloat(GameplayVariables.startTimeResetLive, value);
        }
    }

    public int LastTimeShowRate
    {
        get
        {
            return lastTimeShowRate;
        }
        set
        {
            lastTimeShowRate = value;
            PlayerPrefs.SetInt(GameplayVariables.lastTimeShowRate, value);
        }
    }

    public bool IsRacing
    {
        get
        {
            return isRacing;
        }
        set
        {
            isRacing = value;
            PlayerPrefs.SetInt(GameplayVariables.isRacing, value ? 1 : 0);
        }
    }

    public int AmountRace
    {
        get
        {
            return amountRace;
        }
        set
        {
            amountRace = value;
            PlayerPrefs.SetInt(GameplayVariables.amountRace, value);
        }
    }

    public int LevelStartRace
    {
        get
        {
            return levelStartRace;
        }
        set
        {
            levelStartRace = value;
            PlayerPrefs.SetInt(GameplayVariables.levelStartRace, value);
        }
    }

    public int MyOrderInRace
    {
        get
        {
            return myOrderInRace;
        }
        set
        {
            myOrderInRace = value;
            PlayerPrefs.SetInt(GameplayVariables.myOrderInRace, value);
        }
    }

    public System.DateTime StartTimeRace
    {
        get
        {
            return startTimeRace;
        }
        set
        {
            startTimeRace = value;
            PlayerPrefs.SetString(GameplayVariables.startTimeRace, startTimeRace.ToString());
        }
    }

    public int NextLevel
    {
        get
        {
            return nextLevel;
        }
        set
        {
            nextLevel = value;
            PlayerPrefs.SetInt(GameplayVariables.nextLevel, value);
        }
    }
    
    public int PlayCount
    {
        get
        {
            return playCount;
        }
        set
        {
            playCount = value;
            PlayerPrefs.SetInt(GameplayVariables.playCount, value);
        }
    }
    
    public int LoseCount
    {
        get
        {
            return loseCount;
        }
        set
        {
            loseCount = value;
            PlayerPrefs.SetInt(GameplayVariables.loseCount, value);
        }
    }

    public int AmountWinStreak
    {
        get
        {
            return amountWinStreak;
        }
        set
        {
            if (value > 5)
            {
                value = 5;
            }
            amountWinStreak = value;
            PlayerPrefs.SetInt(GameplayVariables.amountWinStreak, value);
        }
    }

    public int AmountOpen
    {
        get
        {
            return amountOpen;
        }
        set
        {
            amountOpen = value;
            PlayerPrefs.SetInt(GameplayVariables.amountOpen, value);
        }
    }

    public int AmountLevelChestReceived
    {
        get
        {
            return amountLevelChestReceived;
        }
        set
        {
            amountLevelChestReceived = value;
            PlayerPrefs.SetInt(GameplayVariables.amountLevelChestReceived, value);
        }
    }

    public System.DateTime StartTimePass
    {
        get
        {
            return startTimePass;
        }
        set
        {
            startTimePass = value;
            PlayerPrefs.SetString(GameplayVariables.startTimePass, startTimePass.ToString());
        }
    }

    public int DayPass
    {
        get
        {
            return dayPass;
        }
        set
        {
            dayPass = value;
            PlayerPrefs.SetInt(GameplayVariables.dayPass, value);
        }
    }

    public int SkinUsing
    {
        get
        {
            return skinUsing;
        }
        set
        {
            skinUsing = value;
            PlayerPrefs.SetInt(GameplayVariables.skinUsing, value);
        }
    }

    private int bestLevel;
    private int curLevel;
    private int nextLevel;
    private int lastTimeShowRate;
    private int coin;
    private int ticket;
    private int live;
    private bool rate;
    private bool music;
    private bool sound;
    private bool vibrate;
    private bool hasAds;
    private bool hasInfinityLive;
    private int dailyLogin;
    private int totalSpend;
    private int totalEarn;
    private int winLevel;
    private float loseProgress;
    private int levelShowRemoveAds;
    private int amountExtraSlot;
    private int amountUndo;
    private int amountSwap;
    private int amountMagnet;
    private int amountHammer;
    private int amountOpen;
    private int amountInter;
    private int amountRw;
    private int amountLevelChestReceived;
    private float startTimeResetEnergy;
    private int amountWinStreak;
    private int playCount;
    private int loseCount;

    private int skinUsing;

    #region Vip Pass
    private int dayPass = 0;
    private System.DateTime startTimePass;
    #endregion

    #region Race private variables
    private int levelStartRace;
    private bool isRacing;
    private int amountRace;
    private int myOrderInRace;
    private System.DateTime startTimeRace;
    #endregion

    #region Hide In Inspecter
    [HideInInspector]
    public int amountPlay = 0;
    [HideInInspector]
    public int amountReturnToHome = 0;
    [HideInInspector]
    public bool isLoading = true;
    [HideInInspector]
    public bool isNewDay = false;
    [HideInInspector]
    public bool isNewUser = false;
    [HideInInspector]
    public bool isRewardLive = false;
    [HideInInspector]
    public bool hasShowRate = false;
    [HideInInspector]
    public bool hasBuyStarterPack = false;
    [HideInInspector]
    public bool hasBuyRemoveAds = false;
    [HideInInspector]
    public bool hasBuyDeal1 = false;
    [HideInInspector]
    public bool hasBuyDeal2 = false;
    [HideInInspector]
    public bool hasBuy3DayPassPack = false;
    [HideInInspector]
    public bool hasBuy7DayPassPack = false;
    [HideInInspector]
    public bool hasBuy30DayPassPack = false;
    [HideInInspector]
    public List<int> skinOwned = new List<int>();
    [HideInInspector]
    public List<int> listNextLevels = new List<int>();
    [HideInInspector]
    public List<int> screwOwned = new List<int>();
    [HideInInspector]
    public List<int> bgOwned = new List<int>();
    [HideInInspector]
    public List<int> tableOwned = new List<int>();
    #endregion

    public void Init()
    {
        if (PlayerPrefs.GetInt("IsFirstPlay") == 0)
        {
            PlayerPrefs.SetInt("IsFirstPlay", 1);
            isNewUser = true;
        }
        isLoading = true;

        bestLevel = PlayerPrefs.GetInt(GameplayVariables.bestLevel, 1);
        nextLevel = PlayerPrefs.GetInt(GameplayVariables.nextLevel, -1);
        coin = PlayerPrefs.GetInt(GameplayVariables.coin, 1000);
        ticket = PlayerPrefs.GetInt(GameplayVariables.ticket, 0);
        //live = PlayerPrefs.GetInt(GameplayVariables.live, GameManager.MAX_LIVE);
        amountInter = PlayerPrefs.GetInt(GameplayVariables.amountInter, 0);
        amountRw = PlayerPrefs.GetInt(GameplayVariables.amountRw, 0);
        dailyLogin = PlayerPrefs.GetInt(GameplayVariables.dailyLogin, 0);
        totalSpend = PlayerPrefs.GetInt(GameplayVariables.totalSpend, 0);
        totalEarn = PlayerPrefs.GetInt(GameplayVariables.totalEarn, 0);
        winLevel = PlayerPrefs.GetInt(GameplayVariables.winLevel, 0);
        loseProgress = PlayerPrefs.GetFloat(GameplayVariables.loseProgress, 0);
        lastTimeShowRate = PlayerPrefs.GetInt(GameplayVariables.lastTimeShowRate, 0);
        amountExtraSlot = PlayerPrefs.GetInt(GameplayVariables.amountExtraSlot, 2);
        amountUndo = PlayerPrefs.GetInt(GameplayVariables.amountUndo, 2);
        amountSwap = PlayerPrefs.GetInt(GameplayVariables.amountSwap, 2);
        amountMagnet = PlayerPrefs.GetInt(GameplayVariables.amountMagnet, 2);
        amountWinStreak = PlayerPrefs.GetInt(GameplayVariables.amountWinStreak, 0);
        amountOpen = PlayerPrefs.GetInt(GameplayVariables.amountOpen, 0);
        amountLevelChestReceived = PlayerPrefs.GetInt(GameplayVariables.amountLevelChestReceived, 0);
        startTimeResetEnergy = PlayerPrefs.GetFloat(GameplayVariables.startTimeResetLive);
        levelShowRemoveAds = PlayerPrefs.GetInt(GameplayVariables.levelShowRemoveAds, 0);
        rate = PlayerPrefs.GetInt(GameplayVariables.rate, 0) == 1 ? true : false;
        music = PlayerPrefs.GetInt(GameplayVariables.music, 1) == 1 ? true : false;
        sound = PlayerPrefs.GetInt(GameplayVariables.sound, 1) == 1 ? true : false;
        vibrate = PlayerPrefs.GetInt(GameplayVariables.vibrate, 1) == 1 ? true : false;
        hasAds = PlayerPrefs.GetInt(GameplayVariables.hasAds, 1) == 1 ? true : false;
        hasInfinityLive = PlayerPrefs.GetInt(GameplayVariables.hasInfinityLive, 0) == 1 ? true : false;
        playCount = PlayerPrefs.GetInt(GameplayVariables.playCount, 0);
        loseCount = PlayerPrefs.GetInt(GameplayVariables.loseCount, 0);

        skinUsing = PlayerPrefs.GetInt(GameplayVariables.skinUsing, 0);
        curLevel = bestLevel;

        AmountOpen++;

        int[] tempNextLevels = PlayerPrefsX.GetIntArray(GameplayVariables.ListNextLevels);
        if (tempNextLevels.Length <= GameConfig.MAX_LEVEL_RANDOM)
        {
            for (int i = 0; i < tempNextLevels.Length; i++)
            {
                listNextLevels.Add(tempNextLevels[i]);
            }
        }
        else
        {
            PlayerPrefsX.SetIntArray(GameplayVariables.ListNextLevels, listNextLevels.ToArray());
        }
    }

    public int RandomNextLevel()
    {
        int prefab = 10;

        if (listNextLevels.Count >= GameConfig.MAX_LEVEL_RANDOM - 1)
        {
            listNextLevels.Clear();
        }

        List<int> tempLevelRd = new List<int>();
        for (int i = 11; i <= GameManager.MAX_LEVEL; i++)
        {
            int tempCount = 0;
            for (int j = 0; j < listNextLevels.Count; j++)
            {
                if (i == listNextLevels[j])
                {
                    break;
                }
                else
                {
                    tempCount++;
                }
            }

            if (tempCount == listNextLevels.Count)
            {
                tempLevelRd.Add(i);
            }
        }

        prefab = tempLevelRd[Random.Range(0, tempLevelRd.Count)];
        listNextLevels.Add(prefab);
        PlayerPrefsX.SetIntArray(GameplayVariables.ListNextLevels, listNextLevels.ToArray());

        return prefab;
    }

    public bool CanShowDailyBonus()
    {
        bool canShowDaily = false;
        if (DailyLogin == 0)
        {
            //CGTeamBridge.instance.SetPropertyDayPlayed((DailyLogin + 1).ToString());
            //CGTeamBridge.instance.SetPropertyRetendDay("0");
            PlayerPrefs.SetInt("retent_type", 0);
            return true;
        }

        System.DateTime lastTimeOpen = System.DateTime.Parse(PlayerPrefs.GetString(GameplayVariables.lastTimeOpen, System.DateTime.Now.ToString()));
        Debug.Log(PlayerPrefs.GetString(GameplayVariables.lastTimeOpen, System.DateTime.Now.ToString()));
        System.DateTime currentTimeDate = System.DateTime.Now.Date;
        int dayOpen = (currentTimeDate - lastTimeOpen).Days;
        if (dayOpen > 0)
        {
            int retend_type = PlayerPrefs.GetInt("retent_type");
            retend_type += dayOpen;
            PlayerPrefs.SetInt("retent_type", retend_type);
            //CGTeamBridge.instance.SetPropertyRetendDay(retend_type.ToString());

            //CGTeamBridge.instance.SetPropertyDayPlayed((DailyLogin + 1).ToString());
            canShowDaily = true;
        }
        return canShowDaily;
    }

    #region Race
    //public bool CanRace()
    //{
    //    int myDistance = BestLevel - LevelStartRace;

    //    if (myDistance >= RemoteConfig.Instance.DistanceRace)
    //    {
    //        return false;
    //    }

    //    int amountRaceComplete = 0;
    //    for (int i = 0; i < GameManager.Instance.infoRace.raceInfos.Length; i++)
    //    {
    //        System.DateTime currentTime = System.DateTime.Now;
    //        int duration = ((int)(currentTime - StartTimeRace).TotalMinutes);

    //        int distance = (int)(duration * GameManager.Instance.infoRace.raceInfos[i].speed);
    //        if (distance >= RemoteConfig.Instance.DistanceRace)
    //        {
    //            amountRaceComplete++;
    //        }
    //    }

    //    if (amountRaceComplete >= 3)
    //    {
    //        return false;
    //    }
    //    return true;
    //}

    //public int GetMyOrderInRace()
    //{
    //    int order = 5;
    //    int myDistance = BestLevel - LevelStartRace;

    //    if (MyOrderInRace > 0)
    //    {
    //        order = MyOrderInRace;
    //    }
    //    else
    //    {
    //        for (int i = GameManager.Instance.infoRace.raceInfos.Length - 1; i >= 0; i--)
    //        {
    //            System.DateTime currentTime = System.DateTime.Now;
    //            int duration = ((int)(currentTime - StartTimeRace).TotalMinutes);

    //            int distance = (int)(duration * GameManager.Instance.infoRace.raceInfos[i].speed);
    //            if (myDistance >= distance)
    //            {
    //                order--;
    //            }
    //            else
    //            {
    //                break;
    //            }
    //        }
    //    }
    //    return order;
    //}

    //public int GetOtherOrderInRace(int ID)
    //{
    //    int order = ID;

    //    System.DateTime currentTime = System.DateTime.Now;
    //    int duration = ((int)(currentTime - StartTimeRace).TotalMinutes);

    //    int distance = (int)(duration * GameManager.Instance.infoRace.raceInfos[ID - 1].speed);
    //    if (distance >= RemoteConfig.Instance.DistanceRace)
    //    {
    //        if (MyOrderInRace > 0)
    //        {
    //            if (order >= MyOrderInRace)
    //            {
    //                order++;
    //            }
    //        }
    //    }
    //    else
    //    {
    //        int myDistance = BestLevel - LevelStartRace;
    //        if (distance <= myDistance)
    //        {
    //            order++;
    //        }
    //    }

    //    return order;
    //}
    #endregion

    public bool HasTurnOffInternet()
    {
        return Application.internetReachability == NetworkReachability.NotReachable;
    }

    #region Skin
    //public void UnlockSkin(int id)
    //{
    //    for (int i = 0; i < skinOwned.Count; i++)
    //    {
    //        if (skinOwned[i] == id)
    //        {
    //            return;
    //        }
    //    }
    //    skinOwned.Add(id);
    //    PlayerPrefsX.SetIntArray(GameplayVariables.skinOwned, skinOwned.ToArray());
    //}

    //public void RemoveSkinUnlocked(int id)
    //{
    //    skinOwned.Remove(id);
    //    PlayerPrefsX.SetIntArray(GameplayVariables.skinOwned, skinOwned.ToArray());
    //}

    //public bool HasSkin(int id)
    //{
    //    for (int i = 0; i < skinOwned.Count; i++)
    //    {
    //        if (skinOwned[i] == id)
    //        {
    //            return true;
    //        }
    //    }
    //    return false;
    //}

    //public bool CanShowTrySkin()
    //{
    //    for (int i = 0; i < GameManager.Instance.dataSkin.dataSkins.Length; i++)
    //    {
    //        if (!HasSkin(i))
    //        {
    //            return true;
    //        }
    //    }
    //    return false;
    //}

    //public bool CanBuyAllSkin()
    //{
    //    for (int i = 0; i < GameManager.Instance.dataSkin.dataSkins.Length; i++)
    //    {
    //        if (!HasSkin(i))
    //        {
    //            return true;
    //        }
    //    }
    //    return false;
    //}

    //public bool CanBuySkin(int ID)
    //{
    //    if (!HasSkin(ID))
    //    {
    //        return true;
    //    }
    //    return false;
    //}
    #endregion

    #region BG
    //public void UnlockBG(int id)
    //{
    //    for (int i = 0; i < bgOwned.Count; i++)
    //    {
    //        if (bgOwned[i] == id)
    //        {
    //            return;
    //        }
    //    }
    //    bgOwned.Add(id);
    //    PlayerPrefsX.SetIntArray(GameplayVariables.bgOwned, bgOwned.ToArray());
    //}

    //public void RemoveBGUnlocked(int id)
    //{
    //    bgOwned.Remove(id);
    //    PlayerPrefsX.SetIntArray(GameplayVariables.bgOwned, bgOwned.ToArray());
    //}

    //public bool HasBG(int id)
    //{
    //    for (int i = 0; i < bgOwned.Count; i++)
    //    {
    //        if (bgOwned[i] == id)
    //        {
    //            return true;
    //        }
    //    }
    //    return false;
    //}

    //public bool CanShowTryBG()
    //{
    //    for (int i = 0; i < GameManager.Instance.dataBG.dataSkins.Length; i++)
    //    {
    //        if (!HasBG(i))
    //        {
    //            return true;
    //        }
    //    }
    //    return false;
    //}

    //public bool CanBuyAllBG()
    //{
    //    for (int i = 0; i < GameManager.Instance.dataBG.dataSkins.Length; i++)
    //    {
    //        if (!HasBG(i))
    //        {
    //            return true;
    //        }
    //    }
    //    return false;
    //}

    //public bool CanBuyBG(int ID)
    //{
    //    if (!HasBG(ID))
    //    {
    //        return true;
    //    }
    //    return false;
    //}
    #endregion
}
