using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class Card : ScriptableObject 
{
    public new string name; //name of god
    public int type; //Aggro(red) = 1, Neutral(green) = 2, Defensive(blue) = 3
    public Color cardColor = new Color(); //(1, 0, 0) = Aggro(red), (0, 1, 0) = Neutral(green), (0, 0, 1) = Defensive(blue)
    
    public string descriptionText; //for gods this will describe what the god represents
    public Sprite artwork; //sprite of the artwork to be displayed onto our card

    public void Print()
    {
        Debug.Log("name: " + name);
        Debug.Log("type: " + type);
        Debug.Log("description: " + descriptionText);
    }


}
