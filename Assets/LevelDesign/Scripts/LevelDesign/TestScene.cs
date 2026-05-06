using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestScene : MonoBehaviour
{
    [SerializeField] GameObject objCamera;
    [SerializeField] GameObject objEventSystem;
    [SerializeField] GameObject objLight;
    [SerializeField] GameObject objLevelDesign;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void OnClickButtonPlayTest()
    {
        if (SceneManager.GetActiveScene().name.Equals("Gameplay")) return;

        StartCoroutine(IE_LoadScenePlay());
    }

    WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
    IEnumerator IE_LoadScenePlay()
    {
        objLevelDesign.SetActive(false);
        objCamera.SetActive(false);
        objEventSystem.SetActive(false);
        objLight.SetActive(false);

        UserConfig.Instance.Init();
        int level = int.Parse(LevelDesign.Instance.UILevelDesign.levelInput.text);
        UserConfig.Instance.CurLevel = level;
        UserConfig.Instance.BestLevel = level;

        var operation = SceneManager.LoadSceneAsync("Gameplay", LoadSceneMode.Additive);
        do
        {
            yield return null;
        }
        while (!operation.isDone);
        
        yield return waitForEndOfFrame;
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Gameplay"));
        yield return waitForEndOfFrame;
        Debug.Log("GetActiveScene: " + SceneManager.GetActiveScene().name);
    }

    public void OnClickButtonLevelDesign()
    {
        if (SceneManager.GetActiveScene().name.Equals("LevelDesign")) return;

        GameManager.Instance.canControl = false;
        StartCoroutine(IE_LoadSceneLevelDesign());
    }

    IEnumerator IE_LoadSceneLevelDesign()
    {
        var operation = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        do
        {
            yield return null;
        }
        while (!operation.isDone);

        objLevelDesign.SetActive(true);
        objCamera.SetActive(true);
        objEventSystem.SetActive(true);
        objLight.SetActive(true);

        yield return waitForEndOfFrame;
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("LevelDesign"));
        yield return waitForEndOfFrame;
        Debug.Log("GetActiveScene: " + SceneManager.GetActiveScene().name);

        if (GameplayController.Instance != null)
        {
            Destroy(GameplayController.Instance.gameObject);
        }
        if (ParkingManager.Instance != null)
        {
            Destroy(ParkingManager.Instance.gameObject);
        }
    }
}
