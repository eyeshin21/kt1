using UnityEngine;
using System.Collections;
using System;

public static class Vibration
{
#if UNITY_ANDROID && !UNITY_EDITOR
    public static AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
    public static AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
    public static AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
#else
    public static AndroidJavaClass unityPlayer;
    public static AndroidJavaObject currentActivity;
    public static AndroidJavaObject vibrator;
#endif

    static float timeVibrate = 0;

    public static void Vibrate()
    {
        if (Time.time >= timeVibrate)
        {
            timeVibrate = Time.time + 0.1f;

            if (isAndroid())
                vibrator.Call("vibrate");
            else
            {
#if UNITY_IPHONE || UNITY_IOS
Handheld.Vibrate();
#endif
            }
        }
    }

    static long currentStrengthVibrate = 0;
    public static void Vibrate(long milliseconds)
    {
        //if (!UserConfig.Instance.Vibrate) return;

        try
        {
            if (Time.time >= timeVibrate)
            {
                timeVibrate = Time.time + 0.1f;
                currentStrengthVibrate = milliseconds;

                if (isAndroid())
                    vibrator.Call("vibrate", milliseconds);
                else
                {
#if UNITY_IPHONE || UNITY_IOS
Handheld.Vibrate();
#endif
                }
            }
            else if (milliseconds > currentStrengthVibrate)
            {
                currentStrengthVibrate = milliseconds;
                Cancel();

                if (isAndroid())
                    vibrator.Call("vibrate", milliseconds);
                else
                {
#if UNITY_IPHONE || UNITY_IOS
            Handheld.Vibrate();
#endif
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    public static void Vibrate(long[] pattern, int repeat)
    {
        if (isAndroid())
            vibrator.Call("vibrate", pattern, repeat);
        else
        {
#if UNITY_IPHONE || UNITY_IOS
Handheld.Vibrate();
#endif
        }
    }

    public static bool HasVibrator()
    {
        return isAndroid();
    }

    public static void Cancel()
    {
        if (isAndroid())
            vibrator.Call("cancel");
    }

    private static bool isAndroid()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
	return true;
#else
        return false;
#endif
    }
}
