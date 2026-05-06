using System.Collections.Generic;
using UnityEngine;

public static class ShuffleExtension
{
    //shuffle arrays:
    public static void Shuffle<T>(this T[] array, int shuffleAccuracy)
    {
        for (int i = 0; i < shuffleAccuracy; i++)
        {
            int randomIndex = Random.Range(1, array.Length);

            T temp = array[randomIndex];
            array[randomIndex] = array[0];
            array[0] = temp;
        }
    }
    //shuffle lists:
    public static void Shuffle<T>(this List<T> list, int shuffleAccuracy)
    {
        for (int i = 0; i < shuffleAccuracy; i++)
        {
            int randomIndex = Random.Range(1, list.Count);

            T temp = list[randomIndex];
            list[randomIndex] = list[0];
            list[0] = temp;
        }
    }

    public static List<T> GetRandomElements<T>(this List<T> list, int n)
    {
        if (n > list.Count)
            throw new System.ArgumentException("Số phần tử cần lấy lớn hơn kích thước list.");

        // Tạo bản sao danh sách để không ảnh hưởng đến list gốc
        List<T> temp = new List<T>(list);

        // Fisher–Yates shuffle
        int count = temp.Count;
        for (int i = count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (temp[i], temp[j]) = (temp[j], temp[i]);
        }

        // Lấy n phần tử đầu tiên sau khi trộn
        return temp.GetRange(0, n);
    }

    public static T[] GetRandomElements<T>(this T[] array, int n)
    {
        if (array == null || array.Length == 0)
            throw new System.ArgumentException("Mảng rỗng hoặc null.");
        if (n > array.Length)
            throw new System.ArgumentException("Số phần tử cần lấy lớn hơn kích thước mảng.");

        // Tạo bản sao để không ảnh hưởng đến mảng gốc
        T[] temp = new T[array.Length];
        System.Array.Copy(array, temp, array.Length);

        // Fisher–Yates shuffle
        for (int i = temp.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (temp[i], temp[j]) = (temp[j], temp[i]);
        }

        // Tạo mảng kết quả chứa n phần tử đầu tiên
        T[] result = new T[n];
        System.Array.Copy(temp, result, n);

        return result;
    }
}