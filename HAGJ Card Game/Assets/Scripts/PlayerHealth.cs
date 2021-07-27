using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public Sprite heart;
    public Sprite empty;

    public GameObject heart1;
    public GameObject heart2;
    public GameObject heart3;

    public void updateHealth()
    {
        if (GameManager.playerObj[0].GetComponent<Base>().godsDestroyed == 0)
        {
            heart1.GetComponent<Image>().sprite = heart;
            heart2.GetComponent<Image>().sprite = heart;
            heart3.GetComponent<Image>().sprite = heart;
        }
        else if (GameManager.playerObj[0].GetComponent<Base>().godsDestroyed == 1)
            heart3.GetComponent<Image>().sprite = empty;
        else if (GameManager.playerObj[0].GetComponent<Base>().godsDestroyed == 2)
            heart2.GetComponent<Image>().sprite = empty;
        else if (GameManager.playerObj[0].GetComponent<Base>().godsDestroyed == 3)
            heart1.GetComponent<Image>().sprite = empty;
    }
}
