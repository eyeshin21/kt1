using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GDCloth : MonoBehaviour
{
    public int x;
    public int y;

    public GameObject cloth2x2;
    public GameObject cloth2x3;
    public GameObject cloth3x2;
    public TextMeshPro[] txtsAmount;

    public void Init(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public void SetUp(ClothType clothType, int clothCount)
    {
        SetClothType(clothType);
        SetClothCount(clothCount);
    }

    public void SetClothType(ClothType clothType)
    {
        cloth2x2.SetActive(false);
        cloth2x3.SetActive(false);
        cloth3x2.SetActive(false);

        switch (clothType)
        {
            case ClothType.TwoxTwo:
                cloth2x2.SetActive(true);
                break;
            case ClothType.TwoxThree:
                cloth2x3.SetActive(true);
                break;
            case ClothType.ThreexTwo:
                cloth3x2.SetActive(true);
                break;
        }
    }

    public void SetClothCount(int count)
    {
        for (int i = 0; i < txtsAmount.Length; i++)
        {
            txtsAmount[i].text = count.ToString();
        }
    }
}
