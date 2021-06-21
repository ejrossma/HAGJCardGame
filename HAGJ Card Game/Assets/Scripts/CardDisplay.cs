using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{

    public Card displayCard;

    public Text nameText;
    public Image artworkImage;
    public Text descriptionText;
    public Image typeBox;

    // Start is called before the first frame update
    void Start()
    {

        displayCard.Print();
        nameText.text = displayCard.name;
        artworkImage.sprite = displayCard.artwork;
        descriptionText.text = displayCard.descriptionText;
        typeBox.color = displayCard.cardColor;
            
    }

    void Display(Card test)
    {
        nameText.text = test.name;
        artworkImage.sprite = test.artwork;
        descriptionText.text = test.descriptionText;
        typeBox.color = test.cardColor;
    }

}
