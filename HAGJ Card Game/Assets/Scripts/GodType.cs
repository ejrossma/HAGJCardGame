using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GodType : MonoBehaviour
{
    public Image typeImage;

    public void displayType(int type)
    {
        if (type == 1)
        {
            typeImage.color = new Color32(255, 0, 0, 100);
        }
        else if (type == 2)
        {
            typeImage.color = new Color32(0, 255, 0, 100);
        }
        else
        {
            typeImage.color = new Color32(0, 0, 255, 100);
        }
    }
}
