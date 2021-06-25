using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{

    public Card displayCard;

    public Text nameText;
    public Image artworkImage;
    public Text descriptionText;
    public Image typeBox;

    public bool selected; //if the god has been selected to be used

    // Start is called before the first frame update
    void Start()
    {

        //displayCard.Print();
        nameText.text = displayCard.name;
        artworkImage.sprite = displayCard.artwork;
        descriptionText.text = displayCard.descriptionText;
        typeBox.color = displayCard.cardColor;
            
    }

    public void Display(Card test)
    {
        nameText.text = test.name;
        artworkImage.sprite = test.artwork;
        descriptionText.text = test.descriptionText;
        typeBox.color = test.cardColor;
    }

    private void OnMouseDown()
    {
        int playerMaat = GameManager.playerObj[0].GetComponent<Base>().maat;
        Debug.Log("Maat Remaining: " + playerMaat-- + " State: " + GameManager.state + " Action: " + displayCard.action);
        if (!displayCard.destroyed && GameManager.state == GameManager.State.GodSelection)
        {
            selected = true;

            Debug.Log("name: " + displayCard.name);
            Debug.Log("type: " + displayCard.type);
            Debug.Log("description: " + displayCard.descriptionText);
        }
        else if (displayCard.action && GameManager.state == GameManager.State.ActionSelection && playerMaat > 0) //if the card is an action && its action selection time && the player has more than 0 maat
        {
            selected = true;
            Debug.Log("Action Selected \n Maat Remaining: " + playerMaat--);
        }
    }
}
