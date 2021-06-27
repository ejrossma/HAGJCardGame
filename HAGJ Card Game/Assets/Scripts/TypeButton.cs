using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TypeButton : MonoBehaviour
{

    public bool selected;
    public int type;

    public Sprite red;
    public Sprite green;
    public Sprite blue;

    public GameObject typeImage;

    void Start()
    {
        if (type == 1)
        {
            typeImage.GetComponent<Image>().sprite = red;
        }
        else if (type == 2)
        {
            typeImage.GetComponent<Image>().sprite = green;
        }
        else if (type == 3)
        {
            typeImage.GetComponent<Image>().sprite = blue;
        }
    }

    private void OnMouseDown()
    {
        selected = true;
    }

    public void confirmType()
    {
        GameManager.Player.type = type;
    }
}
