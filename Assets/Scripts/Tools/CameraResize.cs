using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraResize : Singleton<CameraResize>
{
    [SerializeField] private float defaultSize = 9.6f;
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

    //private void Start()
    //{
    //    ResizeCamera(defaultSize);
    //}

    public void ResizeCamera(float size)
    {
        defaultSize = size;

        //float height = (float)Screen.height;
        //float width = (float)Screen.width;

        //float ratio = width / height;
        //float ratioDefault = 720.0f / 1280.0f;
        //if (ratio < ratioDefault)
        //{
        //    float diff = (ratio / ratioDefault);
        //    Debug.Log(diff);
        //    cam.orthographicSize = size / diff;
        //}
        //else
        //{
        //    cam.orthographicSize = size;
        //}

        cam.orthographicSize = size;
    }
}
