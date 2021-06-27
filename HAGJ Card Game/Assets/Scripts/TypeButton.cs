using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeButton : MonoBehaviour
{

    public bool selected;
    public int type;

    private void OnMouseDown()
    {
        selected = true;
    }

    public void confirmType()
    {
        GameManager.Player.type = type;
    }
}
