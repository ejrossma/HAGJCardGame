using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private GameObject[] godDeck;
    private GameObject[] inPlay;
    private GameObject selectedCard;

    // Start is called before the first frame update
    void Start()
    {
        //hide all elements besides choosing card from god deck
        inPlay = GameObject.FindGameObjectsWithTag("InPlay");
        foreach (GameObject god in inPlay)
        {
            god.SetActive(false);
        }

        godDeck = GameObject.FindGameObjectsWithTag("GodDeck");
        selectedCard = godDeck[2];


        //selectedCard.Display(selectedCard.Deck[0]);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
