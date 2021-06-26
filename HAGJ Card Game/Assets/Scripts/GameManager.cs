using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

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

    //ENEMY AI HELPER VARIABLES
    private int lastGod; //1 = red, 2 = green, 3 = blue

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

        foreach (Card god in playerDeck)
        {
            god.destroyed = false;
        }

        foreach (Card god in opponentDeck)
        {
            god.destroyed = false;
        }

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

        lastGod = 0;

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
                    //Opponent chooses
                    opponentGodSelection();
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
                handleAction("none", opponentAction());
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
                        state = State.Busy;
                        handleAction(action.GetComponent<CardDisplay>().displayCard.name, opponentAction());

                        //remove the skip button & action cards
                        foreach (GameObject actions in actionCards)
                        {
                            actions.SetActive(false);
                        }
                        skipButton.SetActive(false);                      

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
            lastGod = Player.type;
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

    private void handleAction(string player, string opponent)
    {
        //handle the actions based on their names
            //destruction

            //color change
    }

    //Opponent selects/displays their action
        //if at 1 god left use an action always
            //if has 2 or 3 gods left use an action based on a percentage
                //winning: 25%
                //tie: 50%
                //losing 75%
    private string opponentAction()
    {
        return "none";
    }


    //Opponent selects/displays their god choice
        //if first round choose a random god
            //else 75% chance to choose opposite of what player picked last round
            //25% chance to be random
                //random number generated 1-100
    private void opponentGodSelection()
    {
        float temp = Random.Range(0.0f, 10.0f);
        Debug.Log("random float value: " + temp);
        if (temp > 75.0f || lastGod == 0) //choose a random god
        {
            float temp2 = Random.Range(0.0f, 1.0f);
            if (temp2 > 0.66f)
            {
                locateAndDisplayOpponentGod(1);
            }
            else if (temp2 > 0.33f)
            {
                locateAndDisplayOpponentGod(2);
            }
            else if (temp2 >= 0.0f)
            {
                locateAndDisplayOpponentGod(3);
            }
        }
        else if (lastGod == 1) //if there is a green god alive best option is to go green
        {
            if (temp < 50.0f && godAlive(2))
            {
                locateAndDisplayOpponentGod(2);
            }
            else if (godAlive(3))
            {
                locateAndDisplayOpponentGod(3);
            }
            else
            {
                locateAndDisplayOpponentGod(1);
            }
        }
        else if (lastGod == 2 && godAlive(3)) //if there is a blue god alive best option is to go blue
        {
            if (temp < 50.0f && godAlive(3))
            {
                locateAndDisplayOpponentGod(3);
            }
            else if (godAlive(1))
            {
                locateAndDisplayOpponentGod(1);
            }
            else
            {
                locateAndDisplayOpponentGod(2);
            }
        }
        else if (lastGod == 3 && godAlive(1)) //if there is a red god alive best option is to go red
        {
            if (temp < 50.0f && godAlive(1))
            {
                locateAndDisplayOpponentGod(1);
            }
            else if (godAlive(2))
            {
                locateAndDisplayOpponentGod(2);
            }
            else
            {
                locateAndDisplayOpponentGod(3);
            }
        }
    }

    //check if opponent godDeck has any gods left of the current type
        //if so return true
        //else return false
    private bool godAlive(int type)
    {
        foreach (Card god in opponentDeck)
        {
            if (god.type == type && !god.destroyed)
            {
                return true;
            }
        }
        return false;
    }
    //take in the current type and go through the god deck until running into 1 that matches & isnt destroyed
        //change the inplay[1] to that god and call the display method to update the visual
    private void locateAndDisplayOpponentGod(int type)
    {
        foreach (Card god in opponentDeck)
        {
            if (god.type == type && !god.destroyed)
            {
                inPlay[1].GetComponent<CardDisplay>().displayCard = god;
                inPlay[1].GetComponent<CardDisplay>().Display(inPlay[1].GetComponent<CardDisplay>().displayCard);

                return;
            }
        }
    }
}
