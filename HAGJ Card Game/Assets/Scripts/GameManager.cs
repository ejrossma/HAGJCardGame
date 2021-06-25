using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private GameObject[] godDeck;
    private GameObject[] inPlay;
    private GameObject[] actionCards;

    static public GameObject[] playerObj;
    static public GameObject[] opponentObj;

    public Card[] playerDeck;
    public Card[] opponentDeck;
    public Card[] actionDeck;

    private GameObject skipButton;

    private bool skipped;

    static public State state;
    /* TURN ROTATION FOR STATES
     * 
     * Busy (to set up board)
     * God Selection
     * Busy (AI God Selection & Reveal of Chosen Gods)
     * Action Selection
     * Busy (AI Action Selection & Reveal of Chosen Actions)
     * Joust (then return to top of rotation)
     */
    public enum State
    {
        Busy,
        GodSelection,
        ActionSelection,
        Joust,
        Reset,
    }

    // Start is called before the first frame update
    void Start()
    {
        //set initial state
        state = State.GodSelection;

        //hide all elements besides choosing card from god deck
        inPlay = GameObject.FindGameObjectsWithTag("InPlay");
        foreach (GameObject god in inPlay)
        {
            god.SetActive(false);
        }
        actionCards = GameObject.FindGameObjectsWithTag("Action");
        foreach (GameObject action in actionCards)
        {
            action.SetActive(false);
        }
        GameObject[] temp = GameObject.FindGameObjectsWithTag("SkipButton");
        skipButton = temp[0];
        skipButton.SetActive(false);

        godDeck = GameObject.FindGameObjectsWithTag("GodDeck");

        playerObj = GameObject.FindGameObjectsWithTag("Player");
        opponentObj = GameObject.FindGameObjectsWithTag("Opponent");

        playerDeck = playerObj[0].GetComponent<Base>().godDeck;
        opponentDeck = opponentObj[0].GetComponent<Base>().godDeck;

    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.GodSelection)
        {
            foreach (GameObject god in godDeck)
            {
                //need to add text to tell player to "Select a God"
                if (god.GetComponent<CardDisplay>().selected)
                {
                    state = State.Busy;
                    //change the god card for the player -> the one inside of the 'god' gameobject
                    //then display the new one
                    inPlay[0].GetComponent<CardDisplay>().displayCard = god.GetComponent<CardDisplay>().displayCard;
                    inPlay[0].GetComponent<CardDisplay>().Display(inPlay[0].GetComponent<CardDisplay>().displayCard);
                    //swap from god selection to action selection
                    foreach (GameObject gods in inPlay)
                    {
                        gods.SetActive(true);
                    }
                    foreach (GameObject action in actionCards)
                    {
                        action.SetActive(true);
                    }
                    skipButton.SetActive(true);
                    foreach (GameObject gods in godDeck)
                    {
                        gods.SetActive(false);
                    }
                    god.GetComponent<CardDisplay>().selected = false;

                    //show the correct action cards
                    updateActionCards(inPlay[0].GetComponent<CardDisplay>().displayCard.type);

                    state = State.ActionSelection;
                    Debug.Log("State: " + state);
                }
            }
        }
        else if (state == State.ActionSelection)
        {
            //display "Select an Action or Skip" && 
            //hide "Select a God"

            if (skipped)
            {
                state = State.Busy;
                //remove the skip button & action cards
                foreach (GameObject action in actionCards)
                {
                    action.SetActive(false);
                }
                skipButton.SetActive(false);
                //RESOLVE OPPONENT ACTION HERE BUT FOR NOW JUST SKIP AND GO STRAIHGT TO JOUST
                state = State.Joust; //player skipped their action go to Joust
                skipped = false; //swap back to false for next time through
            } 
            else
            {
                //foreach loop checking each action card to see if they were selected
                foreach (GameObject action in actionCards)
                {
                    if (action.GetComponent<CardDisplay>().selected)
                    {
                        handleAction(action.GetComponent<CardDisplay>().displayCard.name, opponentAction());

                        //remove the skip button & action cards
                        foreach (GameObject actions in actionCards)
                        {
                            actions.SetActive(false);
                        }
                        skipButton.SetActive(false);
                        //do the action
                        Debug.Log("Action Registered as Seleceted");
                        state = State.Busy;

                        action.GetComponent<CardDisplay>().selected = false;

                        //if both gods arent destroyed yet continue onto the joust
                        state = State.Joust;

                    }
                }

            }
        }
        else if (state == State.Joust)
        {
            //Player and Opponent God Information
            Card Player = inPlay[0].GetComponent<CardDisplay>().displayCard;
            Card Opponent = inPlay[1].GetComponent<CardDisplay>().displayCard;
            Debug.Log("3 Beats 1, 1 Beats 2, 2 Beats 3 \n"
                      + "Player: " + Player.type + " vs. Opponent: " + Opponent.type);

            if (Player.type == Opponent.type)
            {
                Debug.Log("Tie");
                state = State.Reset;
            }
            else if ((Player.type == 3 && Opponent.type == 1) || (Player.type == 1 && Opponent.type == 2) || (Player.type == 2 && Opponent.type == 3))
            {
                Debug.Log("Player Wins");
                //need to destroy the copy in their deck not the one that is being referenced
                destroyGod(opponentDeck, Opponent.name, false);
            }
            else
            {
                Debug.Log("Opponent Wins");
                //need to destroy the copy in their deck not the one that is being referenced
                destroyGod(playerDeck, Player.name, true);
            }
        }
        else if (state == State.Reset)
        {
            foreach (GameObject gods in inPlay)
            {
                gods.SetActive(false);
            }
            foreach (GameObject action in actionCards)
            {
                action.SetActive(false);
            }
            skipButton.SetActive(false);
            foreach (GameObject gods in godDeck)
            {
                gods.SetActive(true);
            }
            state = State.GodSelection;
        }
    }

    public void setSkipped()
    {
        skipped = true;
    }

    private void destroyGod(Card[] deck, string godName, bool player)
    {
        foreach (Card god in deck)
        {
            if (godName == god.name)
            {
                god.destroyed = true;
                if (player)
                {
                    playerObj[0].GetComponent<Base>().godsDestroyed++;
                    Debug.Log("Player Gods Destroyed: " + playerObj[0].GetComponent<Base>().godsDestroyed);
                }
                else
                {
                    opponentObj[0].GetComponent<Base>().godsDestroyed++;
                    Debug.Log("Opponent Gods Destroyed: " + opponentObj[0].GetComponent<Base>().godsDestroyed);
                }
                state = State.Reset;
            }
        }
    }

    private void updateActionCards(int type)
    {
        if (type == 1) //show red cards [0], [1]
        {
            actionCards[0].GetComponent<CardDisplay>().displayCard = actionDeck[0];
            actionCards[0].GetComponent<CardDisplay>().Display(actionCards[0].GetComponent<CardDisplay>().displayCard);
            actionCards[1].GetComponent<CardDisplay>().displayCard = actionDeck[1];
            actionCards[1].GetComponent<CardDisplay>().Display(actionCards[1].GetComponent<CardDisplay>().displayCard);
        }
        else if (type == 2) //show green cards [2], [3]
        {
            actionCards[0].GetComponent<CardDisplay>().displayCard = actionDeck[2];
            actionCards[0].GetComponent<CardDisplay>().Display(actionCards[0].GetComponent<CardDisplay>().displayCard);
            actionCards[1].GetComponent<CardDisplay>().displayCard = actionDeck[3];
            actionCards[1].GetComponent<CardDisplay>().Display(actionCards[1].GetComponent<CardDisplay>().displayCard);
        }
        else if (type == 3) //show blue cards [4], [5]
        {
            actionCards[0].GetComponent<CardDisplay>().displayCard = actionDeck[4];
            actionCards[0].GetComponent<CardDisplay>().Display(actionCards[0].GetComponent<CardDisplay>().displayCard);
            actionCards[1].GetComponent<CardDisplay>().displayCard = actionDeck[5];
            actionCards[1].GetComponent<CardDisplay>().Display(actionCards[1].GetComponent<CardDisplay>().displayCard);
        }
    }
}
