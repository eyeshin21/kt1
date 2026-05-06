using Lean.Touch;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TigerForge;
using UnityEngine;

public class GameplayController : Singleton<GameplayController>
{
    [SerializeField] private LeanTouch leanTouch;

    [Header("Camera")]
    [SerializeField] private Camera _cam;
    public Camera cam
    {
        get
        {
            if (_cam == null)
            {
                _cam = Camera.main;
            }

            return _cam;
        }
    }

    [Header("Canvas")]
    [SerializeField] private Canvas _canvas;
    public Canvas canvas
    {
        get
        {
            return _canvas;
        }
    }

    [Header("Level")]
    public LevelController levelController;
    public LayerMask shapeLayer;
    public GameObject holePrefab;
    public Transform board;
    public SpriteRenderer bg;
    public Sprite[] backSprs;

    [HideInInspector] public bool doneLoadLevel;
    private int amountTap = 0;

    private void OnEnable()
    {
        LeanTouch.OnFingerDown += HandleFingerDown;
        LeanTouch.OnFingerTap += HandleFingerTap;
    }

    private void OnDisable()
    {
        LeanTouch.OnFingerDown -= HandleFingerDown;
        LeanTouch.OnFingerTap -= HandleFingerTap;
    }

    // Start is called before the first frame update
    void Start()
    {
        EventManager.StartListening(EventVariables.RecycleLevel, OnReplay);

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        leanTouch.ReferenceDpi = 20;
        leanTouch.TapThreshold = 0.1f;
#else
        leanTouch.ReferenceDpi = 200;
        leanTouch.TapThreshold = 0.2f;
#endif
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameManager.Instance.EndGame(true);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            UserConfig.Instance.CurLevel++;
            UIManager.Instance.ingameMenu.PressedReplayBtn();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            UserConfig.Instance.CurLevel--;
            UIManager.Instance.ingameMenu.PressedReplayBtn();
        }
    }
#endif

    void HandleFingerDown(LeanFinger finger)
    {
        if (finger.IsOverGui || !GameManager.Instance.canControl) return;
    }

    void HandleFingerTap(LeanFinger finger)
    {
        if (finger.IsOverGui || !GameManager.Instance.canControl) return;

        Ray ray = finger.GetRay();
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.transform.TryGetComponent(out ShooterController shooter))
            {
                /*
                if (UserConfig.Instance.CurLevel == 1)
                {
                    if (ShooterManager.Instance.shooters.IndexOf(shooter) == 0)
                    {
                        shooter.OnTap();
                        amountTap++;
                        SoundManager.instance.PlaySound("Select");
                        HapticFeedbackController.TriggerHaptics(MoreMountains.NiceVibrations.HapticTypes.LightImpact);

                        CGTeamBridge.Instance.TrackTutAction($"{amountTap}_hook");
                        if (amountTap == 6)
                        {
                            UIManager.Instance.ingameMenu.tutorialUI.Hide();
                            CGTeamBridge.Instance.TrackTutAction("finish");
                        }
                        else
                        {
                            UIManager.Instance.ingameMenu.tutorialUI.ShowHandTutorial("Tap to Select", Vector2.zero, Vector2.zero, new Vector2(0, -680f), CanvasPositioningExtensions.WorldToCanvasPosition(ShooterManager.Instance.shooters[0].transform.position), 0);
                        }
                    }
                    return;
                }
                */
                if (UserConfig.Instance.CurLevel == 1)
                {
                    if (amountTap == 0)
                    {
                        amountTap++;
                        UIManager.Instance.ingameMenu.tutorialUI.Hide();
                    }
                }
                shooter.OnTap();
                SoundManager.instance.PlaySound("Select");
                HapticFeedbackController.TriggerHaptics(MoreMountains.NiceVibrations.HapticTypes.LightImpact);
            }
            else if (hit.collider.transform.parent.TryGetComponent(out ParkingSlot parkingSlot))
            {
                UIManager.Instance.ShowBuyItemMenu(eBooster.AddSlot);
                EventManager.SetData(EventVariables.AddSlot, parkingSlot);

                SoundManager.instance.PlaySound("Button");
                HapticFeedbackController.TriggerHaptics(MoreMountains.NiceVibrations.HapticTypes.LightImpact);
            }
        }
    }

    public void LoadLevel()
    {
        StartCoroutine(IE_LoadLevel());
    }

    WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
    WaitForSeconds waitForWarning = new WaitForSeconds(1.5f);

    IEnumerator IE_LoadLevel()
    {
        yield return waitForEndOfFrame;

        int level = GameManager.Instance.prefabLevel;

        TextAsset data = Resources.Load<TextAsset>($"LevelData/{level}");
        LevelData levelData;
        if (data.text.Contains("shooterGridSize"))
        {
            levelData = JsonConvert.DeserializeObject<LevelData>(data.text);
        }
        else
        {
            levelData = JsonConvert.DeserializeObject<LevelData>(SaveSystem.Decrypt(System.Convert.ToBase64String(data.bytes), "DeoDucDu0cD@u"));
        }

        /*
        if (cam.aspect >= 0.5625f)
        {
            if (levelData.shooterGridSize.y < 4)
            {
                CameraResize.Instance.ResizeCamera(8.5f);
                cam.transform.position = new Vector3(0f, 2.65f, -35f);
            }
            else if (levelData.shooterGridSize.y < 5)
            {
                CameraResize.Instance.ResizeCamera(9f);
                cam.transform.position = new Vector3(0f, 2.35f, -35f);
            }
            else if (levelData.shooterGridSize.y < 6)
            {
                CameraResize.Instance.ResizeCamera(9.6f);
                cam.transform.position = new Vector3(0f, 1.75f, -35f);
            }
            else if (levelData.shooterGridSize.y < 7)
            {
                CameraResize.Instance.ResizeCamera(10.2f);
                cam.transform.position = new Vector3(0f, 1.2f, -35f);
            }
            else if (levelData.shooterGridSize.y < 8)
            {
                CameraResize.Instance.ResizeCamera(10.8f);
                cam.transform.position = new Vector3(0f, 0.7f, -35f);
            }
            else if (levelData.shooterGridSize.y < 9)
            {
                CameraResize.Instance.ResizeCamera(11.2f);
                cam.transform.position = new Vector3(0f, 0.5f, -35f);
            }
        }
        else
        {
            if (levelData.shooterGridSize.y < 6)
            {
                CameraResize.Instance.ResizeCamera(8.5f);
                cam.transform.position = new Vector3(0f, 2f, -35f);
            }
            if (levelData.shooterGridSize.y < 7)
            {
                CameraResize.Instance.ResizeCamera(8.8f);
                cam.transform.position = new Vector3(0f, 1.5f, -35f);
            }
            else if (levelData.shooterGridSize.y < 8)
            {
                CameraResize.Instance.ResizeCamera(9.2f);
                cam.transform.position = new Vector3(0f, 1f, -35f);
            }
            else if (levelData.shooterGridSize.y < 9)
            {
                CameraResize.Instance.ResizeCamera(9.6f);
                cam.transform.position = new Vector3(0f, 0.5f, -35f);
            }
        }
        */

        int maxSize = 0;
        if (levelData.shooterGridSize.x < levelData.shooterGridSize.y)
        {
            maxSize = levelData.shooterGridSize.y;
        }
        else
        {
            maxSize = levelData.shooterGridSize.x;
        }

        CameraResize.Instance.ResizeCamera(maxSize <= 6 ? 8.5f : (8.5f + 0.5f * (maxSize - 6)));
        cam.transform.position = new Vector3(0f, maxSize <= 6 ? 1.2f : 1.2f - 0.5f * (maxSize - 6), -35f);

        /*
        if (levelData.shooterGridSize.y <= 6)
        {
            CameraResize.Instance.ResizeCamera(8.5f);
        cam.transform.position = new Vector3(0f, 1.2f, -35f);
        }
        else if (levelData.shooterGridSize.y == 7)
        {
            CameraResize.Instance.ResizeCamera(9f);
            cam.transform.position = new Vector3(0f, 0.7f, -35f);
        }
        else if (levelData.shooterGridSize.y == 8)
        {
            CameraResize.Instance.ResizeCamera(9.5f);
            cam.transform.position = new Vector3(0f, 0.2f, -35f);
        }
        else if (levelData.shooterGridSize.y == 9)
        {
            CameraResize.Instance.ResizeCamera(10.0f);
            cam.transform.position = new Vector3(0f, -0.3f, -35f);
        }
        */

        yield return waitForEndOfFrame;

        levelController = Instantiate(Resources.Load<LevelController>($"Levels/Level_{GameManager.Instance.prefabLevel}"));

        ParkingManager.Instance.Init();
        ShooterManager.Instance.Init(levelData.shooterGridSize.ToVector2Int(), levelData.shooterTileDatas);

        eTypeLevel eTypeLevel = GameManager.Instance.GetTypeLevel(UserConfig.Instance.CurLevel);
        bg.sprite = backSprs[(int)eTypeLevel];

        doneLoadLevel = true;

        yield return new WaitUntil(() => FadeMenu.Instance.fadeOut);

        CheckTut();

        ShooterManager.Instance.CheckActiveShooter();

        GameManager.Instance.canControl = true;

        if (eTypeLevel != eTypeLevel.Normal && eTypeLevel != eTypeLevel.Tutorial)
        {
            UIManager.Instance.ShowNoticeHardMenu();
            yield return waitForWarning;
        }

        CheckShowBoosterAndNewElement();
    }

    public void RecycleLevel()
    {
        ShooterManager.Instance.Recycle();
        ParkingManager.Instance.Recycle();

        if (levelController != null)
        {
            Destroy(levelController.gameObject);
        }
    }

    public void OnReplay()
    {
        GameManager.Instance.canControl = false;
        StopCheckLose();
    }

    public void CheckTut()
    {
        amountTap = 0;

        if (UserConfig.Instance.CurLevel == 1)
        {
            Vector3 shooterPos = canvas.WorldToCanvasPosition(ShooterManager.Instance.shooters[0].transform.position, cam);
            Vector3 centerPos = canvas.WorldToCanvasPosition(ShooterManager.Instance.shooters[4].transform.position, cam) + Vector3.up * 25f;

            Vector3 left = ShooterManager.Instance.GetWorldPos(0, 0) - Vector3.right * (ShooterManager.Instance.cellSize / 2f);
            left = ShooterManager.Instance.transform.TransformPoint(left);
            Vector3 leftPos = canvas.WorldToCanvasPosition(left, cam);

            float sizeX = Mathf.Abs(leftPos.x) * 2;

            Vector3 up = ShooterManager.Instance.GetWorldPos(0, ShooterManager.Instance.gridSize.y - 1) + Vector3.up * (ShooterManager.Instance.cellSize / 2f);
            up = ShooterManager.Instance.transform.TransformPoint(up);
            Vector3 upPos = canvas.WorldToCanvasPosition(up, cam);

            Vector3 down = ShooterManager.Instance.GetWorldPos(0, 0) + Vector3.down * (ShooterManager.Instance.cellSize / 2f);
            down = ShooterManager.Instance.transform.TransformPoint(down);
            Vector3 downPos = canvas.WorldToCanvasPosition(down, cam);

            float sizeY = upPos.y - downPos.y;

            UIManager.Instance.ingameMenu.tutorialUI.ShowHandTutorial("Tap to Select", new Vector2(sizeX, sizeY + 30f), centerPos, new Vector2(0, -680f), shooterPos, 0);
        }
        else
        {
            UIManager.Instance.ingameMenu.tutorialUI.Hide();
        }
    }

    public void Revive()
    {
        if (ParkingManager.Instance.CanAddSlot())
        {
            ParkingManager.Instance.AddSlot();
        }
        else
        {
            ParkingManager.Instance.Clear();
        }

        levelController.OnRevive();

        EventManager.EmitEvent(EventVariables.Revive);
    }

    public void CheckWin()
    {
        if (levelController.screws.Count == 0 && ShooterManager.Instance.shooters.Count == 0 && ParkingManager.Instance.parkedShooters.Count == 0)
        {
            GameManager.Instance.EndGame(true);
        }
    }

    public void CheckLose()
    {
        StopCheckLose();
        coroutineCheckLose = StartCoroutine(IE_CheckLose());
    }

    public void StopCheckLose()
    {
        if (coroutineCheckLose != null)
        {
            StopCoroutine(coroutineCheckLose);
            coroutineCheckLose = null;
        }
    }

    WaitForSeconds waitForCheckLose = new WaitForSeconds(0.5f);
    WaitForSeconds waitForShapeMoving = new WaitForSeconds(1.5f);

    Coroutine coroutineCheckLose;
    IEnumerator IE_CheckLose()
    {
        if (!GameManager.Instance.canControl || ShooterManager.Instance.shooters.Count == 0)// || (!IsParkingSlotFull() && IsAnyShooterCanTap()))
        {
            coroutineCheckLose = null;
            yield break;
        }

        bool isAnyParkedShooterMatchScrew = IsAnyParkedShooterMatchScrew();
        bool isAnyShapeMoving = IsAnyShapeMoving();

#if UNITY_EDITOR
        Debug.Log("---Check Shape Moving---");
        Debug.Log($"IsAnyParkedShooterMatchScrew: {isAnyParkedShooterMatchScrew}");
        Debug.Log($"IsAnyShapeMoving: {isAnyShapeMoving}");
#endif

        if (isAnyParkedShooterMatchScrew && isAnyShapeMoving)
        {
#if UNITY_EDITOR
            Debug.Log("Wait 1.5s");
#endif
            yield return waitForShapeMoving;
        }
        //else
        //{
        //#if UNITY_EDITOR
        //            Debug.Log("Wait 0.5s");
        //#endif
        //yield return waitForCheckLose;
        //}

#if UNITY_EDITOR
        Debug.Log("---Check Lose---");
#endif

        bool isParkingSlotFull = IsParkingSlotFull();
        bool isAnyParkedShooterCanShoot = IsAnyParkedShooterCanShoot();
        bool isAnyShooterCanTap = IsAnyShooterCanTap();

#if UNITY_EDITOR
        Debug.Log($"IsParkingSlotFull: {isParkingSlotFull}");
        Debug.Log($"IsAnyParkedShooterCanShoot: {isAnyParkedShooterCanShoot}");
        Debug.Log($"IsAnyShooterCanTap: {isAnyShooterCanTap}");
#endif

        if (GameManager.Instance.canControl && !isAnyParkedShooterCanShoot && (isParkingSlotFull || !isAnyShooterCanTap))
        {
            levelController.OnLose();
            ParkingManager.Instance.RotateShooter(out float rotateTime);
            GameManager.Instance.EndGame(false, rotateTime);
#if UNITY_EDITOR
            Debug.Log("Lose");
#endif
        }

        coroutineCheckLose = null;
    }

    public bool IsParkingSlotFull()
    {
        int totalFreeParkingSlot = ParkingManager.Instance.TotalFreeParkingSlot();

#if UNITY_EDITOR
        Debug.Log($"TotalFreeParkingSlot: {totalFreeParkingSlot}");
#endif

        return totalFreeParkingSlot == 0;
    }

    public bool IsAnyParkedShooterCanShoot()
    {
        List<ShooterController> parkedShooters = ParkingManager.Instance.parkedShooters;

        if (parkedShooters.Count > 0 && levelController.screwsActive.Count > 0)
        {
            for (int i = 0; i < parkedShooters.Count; i++)
            {
                if (parkedShooters[i].CanShootAnyScrew())
                {
#if UNITY_EDITOR
                    Debug.Log($"{parkedShooters[i].gameObject.name} can shoot");
#endif
                    return true;
                }
            }
        }
        else if (/*parkedShooters.Count == 0 ||*/ levelController.screwsActive.Count == 0)
        {
            return true;
        }

        return false;
    }

    public bool IsAnyParkedShooterMatchScrew()
    {
        List<ShooterController> parkedShooters = ParkingManager.Instance.parkedShooters;

        if (parkedShooters.Count > 0 && levelController.screwsActive.Count > 0)
        {
            Dictionary<ColorEnum, int> screwsDict = new Dictionary<ColorEnum, int>();
            for (int i = 0; i < levelController.screwsActive.Count; i++)
            {
                ScrewController screwActive = levelController.screwsActive[i];
                if (!screwsDict.ContainsKey(screwActive.color))
                {
                    screwsDict.Add(screwActive.color, 0);
                }
                screwsDict[screwActive.color]++;
            }

            for (int i = 0; i < parkedShooters.Count; i++)
            {
                ShooterController parkedShooter = parkedShooters[i];
                if (!parkedShooter.isShooting && parkedShooter.realCapacity > 0)
                {
                    if (screwsDict.ContainsKey(parkedShooter.color) /*&& screwsDict[parkedShooter.color] >= parkedShooter.realCapacity*/)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public bool IsAnyShapeMoving()
    {
        int currentIndex = levelController.currentActiveLayer - 1;

        if (currentIndex >= levelController.layers.Count) return false;

        LevelLayer currentLayer = levelController.layers[currentIndex];
        for (int i = 0; i < currentLayer.shapes.Count; i++)
        {
            ShapeController shape = currentLayer.shapes[i];

            if (shape.rb.bodyType == RigidbodyType2D.Dynamic && shape.isActiveRb)
            {
                return true;
            }
        }

        //int lastIndex = currentIndex - 1;
        //if (lastIndex >= 0)
        //{
        //    LevelLayer lastLayer = levelController.layers[lastIndex];

        //    for (int i = 0; i < lastLayer.shapes.Count; i++)
        //    {
        //        ShapeController shape = lastLayer.shapes[i];

        //        if (shape.gameObject.activeSelf && shape.rb.bodyType == RigidbodyType2D.Dynamic)
        //        {
        //            return true;
        //        }
        //    }
        //}

        return false;
    }

    public bool IsAnyShooterCanTap()
    {
        List<ShooterController> shooters = ShooterManager.Instance.shooters;

        if (shooters.Count > 0)
        {
            for (int i = 0; i < shooters.Count; i++)
            {
                if (shooters[i].CanTap())
                {
                    return true;
                }
            }
        }

        return false;
    }

    void CheckShowBoosterAndNewElement()
    {
        bool showBoosterUndo = false;
        bool showBoosterExtraSlot = false;
        bool showBoosterSwap = false;
        bool showBoosterMagnet = false;

        if (UserConfig.Instance.CurLevel == GameConfig.LEVEL_UNLOCK_BOOSTER_UNDO)
        {
            showBoosterUndo = true;
        }
        if (UserConfig.Instance.CurLevel == GameConfig.LEVEL_UNLOCK_BOOSTER_EXTRA_SLOT)
        {
            showBoosterExtraSlot = true;
        }
        if (UserConfig.Instance.CurLevel == GameConfig.LEVEL_UNLOCK_BOOSTER_SWAP)
        {
            showBoosterSwap = true;
        }
        if (UserConfig.Instance.CurLevel == GameConfig.LEVEL_UNLOCK_BOOSTER_MAGNET)
        {
            showBoosterMagnet = true;
        }

        if (showBoosterUndo && !PlayerPrefs.HasKey("Introduce_" + eTypeBooster.Undo))
        {
            UIManager.Instance.ShowNewBoosterMenu(eTypeBooster.Undo);
            PlayerPrefs.SetInt("Introduce_" + eTypeBooster.Undo, 1);

            ShooterManager.Instance.shooters[6].OnTap();
        }
        else if (showBoosterExtraSlot && !PlayerPrefs.HasKey("Introduce_" + eTypeBooster.ExtraSlot))
        {
            UIManager.Instance.ShowNewBoosterMenu(eTypeBooster.ExtraSlot);
            PlayerPrefs.SetInt("Introduce_" + eTypeBooster.ExtraSlot, 1);

            ShooterManager.Instance.shooters[9].OnTap();
            //ShooterManager.Instance.shooters[19].OnTap();
        }
        else if (showBoosterSwap && !PlayerPrefs.HasKey("Introduce_" + eTypeBooster.Swap))
        {
            UIManager.Instance.ShowNewBoosterMenu(eTypeBooster.Swap);
            PlayerPrefs.SetInt("Introduce_" + eTypeBooster.Swap, 1);

            ShooterManager.Instance.shooters[3].OnTap();
        }
        else if (showBoosterMagnet && !PlayerPrefs.HasKey("Introduce_" + eTypeBooster.Magnet))
        {
            UIManager.Instance.ShowNewBoosterMenu(eTypeBooster.Magnet);
            PlayerPrefs.SetInt("Introduce_" + eTypeBooster.Magnet, 1);

            ShooterManager.Instance.shooters[7].OnTap();
        }

        if (UserConfig.Instance.CurLevel == GameConfig.LEVEL_UNLOCK_ELEMENT_TUNNEL && !PlayerPrefs.HasKey("Introduce_" + eTypeElement.Tunnel))
        {
            UIManager.Instance.ShowNewElementMenu(eTypeElement.Tunnel);
            PlayerPrefs.SetInt("Introduce_" + eTypeElement.Tunnel, 1);
        }
        else if (UserConfig.Instance.CurLevel == GameConfig.LEVEL_UNLOCK_ELEMENT_HIDDEN && !PlayerPrefs.HasKey("Introduce_" + eTypeElement.Hidden))
        {
            UIManager.Instance.ShowNewElementMenu(eTypeElement.Hidden);
            PlayerPrefs.SetInt("Introduce_" + eTypeElement.Hidden, 1);
        }
        else if (UserConfig.Instance.CurLevel == GameConfig.LEVEL_UNLOCK_ELEMENT_LINKED_HOOK && !PlayerPrefs.HasKey("Introduce_" + eTypeElement.LinkedHook))
        {
            UIManager.Instance.ShowNewElementMenu(eTypeElement.LinkedHook);
            PlayerPrefs.SetInt("Introduce_" + eTypeElement.LinkedHook, 1);
        }
        else if (UserConfig.Instance.CurLevel == GameConfig.LEVEL_UNLOCK_ELEMENT_ICE && !PlayerPrefs.HasKey("Introduce_" + eTypeElement.Ice))
        {
            UIManager.Instance.ShowNewElementMenu(eTypeElement.Ice);
            PlayerPrefs.SetInt("Introduce_" + eTypeElement.Ice, 1);
        }
        else if (UserConfig.Instance.CurLevel == GameConfig.LEVEL_UNLOCK_ELEMENT_LOCK_AND_KEY && !PlayerPrefs.HasKey("Introduce_" + eTypeElement.LockAndKey))
        {
            UIManager.Instance.ShowNewElementMenu(eTypeElement.LockAndKey);
            PlayerPrefs.SetInt("Introduce_" + eTypeElement.LockAndKey, 1);
        }
        //else if (UserConfig.Instance.CurLevel == GameConfig.LEVEL_UNLOCK_ELEMENT_CIRCLE_HOOK && !PlayerPrefs.HasKey("Introduce_" + eTypeElement.CircleHook))
        //{
        //    UIManager.Instance.ShowNewElementMenu(eTypeElement.CircleHook);
        //    PlayerPrefs.SetInt("Introduce_" + eTypeElement.CircleHook, 1);
        //}
        //else if (UserConfig.Instance.CurLevel == GameConfig.LEVEL_UNLOCK_ELEMENT_TRIANGLE_HOOK && !PlayerPrefs.HasKey("Introduce_" + eTypeElement.TriangleHook))
        //{
        //    UIManager.Instance.ShowNewElementMenu(eTypeElement.TriangleHook);
        //    PlayerPrefs.SetInt("Introduce_" + eTypeElement.TriangleHook, 1);
        //}
        else if (UserConfig.Instance.CurLevel == GameConfig.LEVEL_UNLOCK_ELEMENT_CRATE && !PlayerPrefs.HasKey("Introduce_" + eTypeElement.Crate))
        {
            UIManager.Instance.ShowNewElementMenu(eTypeElement.Crate);
            PlayerPrefs.SetInt("Introduce_" + eTypeElement.Crate, 1);
        }
        else if (UserConfig.Instance.CurLevel == GameConfig.LEVEL_UNLOCK_ELEMENT_PIN && !PlayerPrefs.HasKey("Introduce_" + eTypeElement.Pin))
        {
            UIManager.Instance.ShowNewElementMenu(eTypeElement.Pin);
            PlayerPrefs.SetInt("Introduce_" + eTypeElement.Pin, 1);
        }
        else if (UserConfig.Instance.CurLevel == GameConfig.LEVEL_UNLOCK_ELEMENT_SHUTTER && !PlayerPrefs.HasKey("Introduce_" + eTypeElement.Shutter))
        {
            UIManager.Instance.ShowNewElementMenu(eTypeElement.Shutter);
            PlayerPrefs.SetInt("Introduce_" + eTypeElement.Shutter, 1);
        }
        else if (UserConfig.Instance.CurLevel == GameConfig.LEVEL_UNLOCK_ELEMENT_CLOTH && !PlayerPrefs.HasKey("Introduce_" + eTypeElement.Cloth))
        {
            UIManager.Instance.ShowNewElementMenu(eTypeElement.Cloth);
            PlayerPrefs.SetInt("Introduce_" + eTypeElement.Cloth, 1);
        }
    }

    #region Editor
#if UNITY_EDITOR
    [ContextMenu("Update Data Level")]
    public void UpdateDataLevel()
    {
        for (int i = 1; i <= 100; i++)
        {
            LevelData levelData = JsonConvert.DeserializeObject<LevelData>(Resources.Load<TextAsset>($"LevelData/{i}").text);
            foreach (var data in levelData.shooterTileDatas)
            {
                foreach (var shooter in data.shooters)
                {
                    shooter.capacity = 4;
                }
            }

            string txt = JsonConvert.SerializeObject(levelData);

            string path = Path.Combine(Application.dataPath, "Resources", "LevelData");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            path = Path.Combine(path, string.Concat(string.Format("{0}", i), ".json"));
            File.WriteAllText(path, txt);

            UnityEditor.AssetDatabase.Refresh();

            Debug.Log(string.Format("Save level {0} success!!!", i));
        }
    }
#endif
    #endregion
}
