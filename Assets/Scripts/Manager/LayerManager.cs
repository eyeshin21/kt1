using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerManager : Singleton<LayerManager>
{
    public LayerMask layer1Check;
    public LayerMask layer2Check;
    public LayerMask layer3Check;
    public LayerMask layer4Check;
    public LayerMask layer5Check;
    public LayerMask layer6Check;
    public LayerMask layer7Check;

    public LayerMask GetLayerCheck(int layer)
    {
        LayerMask layerMask = layer1Check;
        switch (layer)
        {
            case 1:
                layerMask = layer1Check;
                break;
            case 2:
                layerMask = layer2Check;
                break;
            case 3:
                layerMask = layer3Check;
                break;
            case 4:
                layerMask = layer4Check;
                break;
            case 5:
                layerMask = layer5Check;
                break;
            case 6:
                layerMask = layer6Check;
                break;
            case 7:
                layerMask = layer7Check;
                break;
        }

        return layerMask;
    }
}
