using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerFavor : MonoBehaviour
{
    public Sprite gem;
    public Sprite empty;

    public GameObject gem1;
    public GameObject gem2;
    public GameObject gem3;

    public void updateFavor()
    {
        if (GameManager.playerObj[0].GetComponent<Base>().maat == 3)
        {
            gem1.GetComponent<Image>().sprite = gem;
            gem2.GetComponent<Image>().sprite = gem;
            gem3.GetComponent<Image>().sprite = gem;
        }
        else if (GameManager.playerObj[0].GetComponent<Base>().maat == 2)
            gem3.GetComponent<Image>().sprite = empty;
        else if (GameManager.playerObj[0].GetComponent<Base>().maat == 1)
            gem2.GetComponent<Image>().sprite = empty;
        else if (GameManager.playerObj[0].GetComponent<Base>().maat == 0)
            gem1.GetComponent<Image>().sprite = empty;
    }
}
