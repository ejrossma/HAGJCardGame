using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GodType : MonoBehaviour
{
    public Sprite red;
    public Sprite green;
    public Sprite blue;

    public GameObject typeImage;

    public void displayType(bool player)
    {
        if (player)
        {
            if (GameManager.Player.type == 1)
            {
                typeImage.GetComponent<Image>().sprite = red;
            }
            else if (GameManager.Player.type == 2)
            {
                typeImage.GetComponent<Image>().sprite = green;
            }
            else if (GameManager.Player.type == 3)
            {
                typeImage.GetComponent<Image>().sprite = blue;
            }
        }
        else if (!player)
        {
            if (GameManager.Opponent.type == 1)
            {
                typeImage.GetComponent<Image>().sprite = red;
            }
            else if (GameManager.Opponent.type == 2)
            {
                typeImage.GetComponent<Image>().sprite = green;
            }
            else if (GameManager.Opponent.type == 3)
            {
                typeImage.GetComponent<Image>().sprite = blue;
            }
        }

    }
}
