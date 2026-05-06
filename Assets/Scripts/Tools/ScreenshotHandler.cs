using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ScreenshotHandler : MonoBehaviour {

    public static ScreenshotHandler instance;

    private Camera myCamera;
    private bool takeScreenshotOnNextFrame;

    int count = 0;

    private void Awake() {

        instance = this;
    }

    private void Start()
    {
        myCamera = GetComponent<Camera>();
    }

    private void OnPostRender() {
        if (takeScreenshotOnNextFrame) {
            takeScreenshotOnNextFrame = false;
            RenderTexture renderTexture = myCamera.targetTexture;

            Texture2D renderResult = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
            Rect rect = new Rect(0, 0, renderTexture.width, renderTexture.height);
            renderResult.ReadPixels(rect, 0, 0);

            byte[] byteArray = renderResult.EncodeToPNG();
            System.IO.File.WriteAllBytes(Application.persistentDataPath + "/" + count + ".png", byteArray);
            //AssetDatabase.Refresh();
            Debug.Log(Application.persistentDataPath);
            RenderTexture.ReleaseTemporary(renderTexture);
            myCamera.targetTexture = null;
        }
    }

    private void TakeScreenshot(int width, int height) {
        myCamera.targetTexture = RenderTexture.GetTemporary(width, height, 16);
        takeScreenshotOnNextFrame = true;
    }

    public void SaveImage() 
    {
        TakeScreenshot(792, 1019);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            count++;
            SaveImage();
        }
    }
}
