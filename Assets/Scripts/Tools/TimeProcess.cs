using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TimeProcess
{
    public static string ConvertSecondToMinute(float second)
    {
        string minute = "";
        int tempMinute = (int)second / 60;
        if (tempMinute < 10)
        {
            minute = "0" + tempMinute;
        }
        else
        {
            minute = tempMinute.ToString();
        }
        minute += ":";
        int tempSecond = (int)second - tempMinute * 60;
        if (tempSecond < 10)
        {
            minute += "0" + tempSecond;
        }
        else
        {
            minute += tempSecond.ToString();
        }
        return minute;
    }

    public static string ConvertSecondToHour(float second)
    {
        string time = "";
        int tempHour = (int)second / 3600;
        if (tempHour < 10)
        {
            time = "0" + tempHour;
        }
        else
        {
            time = tempHour.ToString();
        }
        time += ":";

        int tempMinute = ((int)second - tempHour * 3600) / 60;
        if (tempMinute < 10)
        {
            time += "0" + tempMinute;
        }
        else
        {
            time += tempMinute.ToString();
        }
        time += ":";

        int tempSecond = (int)second - tempMinute * 60 - tempHour * 3600;
        if (tempSecond < 10)
        {
            time += "0" + tempSecond;
        }
        else
        {
            time += tempSecond.ToString();
        }
        return time;
    }

    public static string ConvertSecondToDay(float second)
    {
        string time = "";
        //string timeHour = "", timeMinute = "";
        int tempDay = (int)second / 86400;
        time = tempDay.ToString();
        time += "d ";

        int tempHour = ((int)second - tempDay * 86400) / 3600;
        //if (tempHour < 10)
        //{
        //    timeHour = "0" + tempHour;
        //}
        //else
        //{
        //    timeHour = tempHour.ToString();
        //}



        int tempMinute = ((int)second - tempDay * 86400 - tempHour * 3600) / 60;
        //if (tempMinute < 10)
        //{
        //    timeMinute = "0" + tempMinute;
        //}
        //else
        //{
        //    timeMinute = tempMinute.ToString();
        //}

        int tempSecond = (int)second - tempDay * 86400 - tempMinute * 60 - tempHour * 3600;
        //if (tempSecond < 10)
        //{
        //    timeSecond = "0" + tempSecond;
        //}
        //else
        //{
        //    timeSecond = tempSecond.ToString();
        //}
        if(tempMinute == 0 && tempSecond > 0)
        {
            tempMinute = 1;
        }

        if (tempHour == 0 && tempMinute > 0)
        {
            tempHour = 1;
        }

        if (tempHour > 0 || tempDay > 0)
        {
            return string.Format("{0}d {1}h", tempDay, tempHour);
        }
        return string.Format("{0}h:{1}m", tempHour, tempMinute);
    }
}
