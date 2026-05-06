using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasAdapter : MonoBehaviour
{
    CanvasScaler canvasScaler;
	Canvas canvas;

    private float scaler;
    private float width;
    private float height;

	private void Start()
	{
        height = (float)Screen.height;
        width = (float)Screen.width;

        canvasScaler = GetComponent<CanvasScaler>();
        float ratio = width / height;
        if (width / height > 720.0f / 1280.0f)
        {
            canvasScaler.matchWidthOrHeight = 1;
        }
        else
        {
            canvasScaler.matchWidthOrHeight = 0;
        }

        canvas = GetComponent<Canvas>();
		canvas.worldCamera = Camera.main;
	}
}
