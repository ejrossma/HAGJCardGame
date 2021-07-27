using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using System.Runtime.InteropServices.ComTypes;

public class GameManager : MonoBehaviour
{
    private GameObject[] godDeck;
    private GameObject[] pGod;
    private GameObject[] oGod;
    private GameObject[] actionCards;
    private GameObject[] typeChange;
    private GameObject[] godType;
    private GameObject[] godDeckSymbols;
    private GameObject[] displayActionCards;
    private GameObject[] favorGems;
    private GameObject[] healthIndicator;

    static public GameObject[] playerObj;
    static public GameObject[] opponentObj;

    public Card[] playerDeck;
    public Card[] opponentDeck;
    public Card[] actionDeck;

    static public Card Player;
    static public Card Opponent;

    private Card selectedAction;
    private Card opponentSelectedAction;

    public Sprite forJoust;
    public Sprite forNext;

    //ENEMY AI HELPER VARIABLE
    private int lastGod; //1 = red, 2 = green, 3 = blue

    private GameObject skipButton;
    private GameObject confirmButton;
    private GameObject resetButton;
    private GameObject joustOrResetButton;
    private GameObject postJoustButton;
    private GameObject winLossButton;
    
    private GameObject playerFavor;
    private GameObject opponentFavor;
    private GameObject playerHealth;
    private GameObject opponentHealth;

    private bool skipped;
    private bool confirmed;
    private bool joustOrReset;
    private bool postJoust;
    private bool osFlip; //flip the joust if osiris's resurrection was activated

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
        ActionDisplay,
        Joust,
        PostJoust,
        Reset,
        GameOver,
    }

    // Start is called before the first frame update
    void Start()
    {
        //set initial state
        state = State.GodSelection;

        playerObj = GameObject.FindGameObjectsWithTag("Player");
        opponentObj = GameObject.FindGameObjectsWithTag("Opponent");

        playerDeck = playerObj[0].GetComponent<Base>().godDeck;
        opponentDeck = opponentObj[0].GetComponent<Base>().godDeck;

        playerFavor = GameObject.Find("CardCanvas/ActionAndJoust/Player/PlayerFavor");
        opponentFavor = GameObject.Find("CardCanvas/ActionAndJoust/Opponent/OpponentFavor");

        playerHealth = GameObject.Find("CardCanvas/ActionAndJoust/Player/PlayerHealth");
        opponentHealth = GameObject.Find("CardCanvas/ActionAndJoust/Opponent/OpponentHealth");

        foreach (Card god in playerDeck)
        {
            god.destroyed = false;
            god.type = god.baseType;
        }

        foreach (Card god in opponentDeck)
        {
            god.destroyed = false;
            god.type = god.baseType;
        }

        //hide all elements besides choosing card from god deck
        pGod = GameObject.FindGameObjectsWithTag("PlayerGod");
        foreach (GameObject god in pGod)
        {
            god.SetActive(false);
        }
        oGod = GameObject.FindGameObjectsWithTag("OpponentGod");
        foreach (GameObject god in oGod)
        {
            god.SetActive(false);
        }
        actionCards = GameObject.FindGameObjectsWithTag("Action");
        foreach (GameObject action in actionCards)
        {
            action.SetActive(false);
        }
        displayActionCards = GameObject.FindGameObjectsWithTag("DisplayAction");
        foreach (GameObject action in displayActionCards)
        {
            action.SetActive(false);
        }
        typeChange = GameObject.FindGameObjectsWithTag("TypeChange");
        foreach (GameObject type in typeChange)
        {
            type.SetActive(false);
        }
        //need to write a script that updates the color of this depending on the god the player chose
        godType = GameObject.FindGameObjectsWithTag("GodType");
        foreach (GameObject type in godType)
        {
            type.SetActive(false);
        }
        //god deck symbols for distinction
        godDeckSymbols = GameObject.FindGameObjectsWithTag("GodDeckSymbols");
        foreach (GameObject type in godDeckSymbols)
        {
            type.SetActive(false);
        }
        godDeck = GameObject.FindGameObjectsWithTag("GodDeck");
        foreach (GameObject god in godDeck)
        {
            god.SetActive(false);
            god.GetComponent<CardDisplay>().Display(god.GetComponent<CardDisplay>().displayCard);
        }
        //favor indicator for player
        favorGems = GameObject.FindGameObjectsWithTag("Gem");
        foreach (GameObject gem in favorGems)
        {
            gem.SetActive(false);
        }
        //health indicator for player
        healthIndicator = GameObject.FindGameObjectsWithTag("Heart");
        foreach (GameObject heart in healthIndicator)
        {
            heart.SetActive(false);
        }
        //skip button
        GameObject[] temp = GameObject.FindGameObjectsWithTag("SkipButton");
        skipButton = temp[0];
        skipButton.SetActive(false);
        //confirm button
        temp = GameObject.FindGameObjectsWithTag("ConfirmButton");
        confirmButton = temp[0];
        confirmButton.SetActive(false);
        //reset button
        temp = GameObject.FindGameObjectsWithTag("ResetButton");
        resetButton = temp[0];
        resetButton.SetActive(false);
        //joustOrReset button
        temp = GameObject.FindGameObjectsWithTag("JoustOrResetButton");
        joustOrResetButton = temp[0];
        joustOrResetButton.SetActive(false);
        //postjoust button
        temp = GameObject.FindGameObjectsWithTag("PostJoustButton");
        postJoustButton = temp[0];
        postJoustButton.SetActive(false);
        //winlossbutton
        temp = GameObject.FindGameObjectsWithTag("WinLossButton");
        winLossButton = temp[0];
        winLossButton.SetActive(false);

        lastGod = 0;

        //Player and Opponent God Information
        Player = pGod[0].GetComponent<CardDisplay>().displayCard;
        Opponent = oGod[0].GetComponent<CardDisplay>().displayCard;

        godType[0].GetComponent<GodType>().displayType(true);
        godType[1].GetComponent<GodType>().displayType(false);

    }

    // Update is called once per frame
    void Update()
    {
        if (opponentObj[0].GetComponent<Base>().godsDestroyed == 3)
        {
            state = State.GameOver;
        }
        if (playerObj[0].GetComponent<Base>().godsDestroyed == 3)
        {
            state = State.GameOver;
        }
        if (state == State.GodSelection)
        {
            foreach (GameObject type in godDeckSymbols)
            {
                type.SetActive(true);
            }
            foreach (GameObject god in godDeck)
            {
                god.SetActive(true);
            }
            foreach (GameObject god in godDeck)
            {
                if (god.GetComponent<CardDisplay>().displayCard.destroyed)
                {
                    god.GetComponent<CardDisplay>().slash.SetActive(true);
                }
                //need to add text to tell player to "Select a God"
                if (god.GetComponent<CardDisplay>().selected)
                {
                    state = State.Busy;
                    //change the god card for the player -> the one inside of the 'god' gameobject
                    //then display the new one
                    Player = god.GetComponent<CardDisplay>().displayCard;
                    pGod[0].GetComponent<CardDisplay>().Display(Player);
                    godType[0].GetComponent<GodType>().displayType(true);
                    //Opponent chooses
                    opponentGodSelection();
                    //swap from god selection to action selection
                    foreach (GameObject gods in oGod)
                    {
                        gods.SetActive(true);
                    }
                    foreach (GameObject gods in pGod)
                    {
                        gods.SetActive(true);
                    }
                    foreach (GameObject action in actionCards)
                    {
                        action.SetActive(true);
                    }
                    foreach (GameObject type in godType)
                    {
                        type.SetActive(true);
                    }
                    foreach (GameObject gem in favorGems)
                    {
                        gem.SetActive(true);
                    }
                    foreach (GameObject heart in healthIndicator)
                    {
                        heart.SetActive(true);
                    }
                    skipButton.SetActive(true);
                    foreach (GameObject gods in godDeck)
                    {
                        gods.SetActive(false);
                    }
                    foreach (GameObject type in godDeckSymbols)
                    {
                        type.SetActive(false);
                    }

                    god.GetComponent<CardDisplay>().selected = false;

                    //show the correct action cards & types
                    updateActionCards(Player.type);
                    updateTypes();
                    state = State.ActionSelection;
                }
            }
        }
        else if (state == State.ActionSelection)
        {
            //display "Select an Action or Skip" && 
            //hide "Select a God"

            pGod[0].GetComponent<CardDisplay>().slash.SetActive(false);
            oGod[0].GetComponent<CardDisplay>().slash.SetActive(false);

            if (skipped)
            {
                state = State.Busy;
                //remove the skip button & action cards
                foreach (GameObject action in actionCards)
                {
                    action.SetActive(false);
                }
                foreach (GameObject actions in displayActionCards)
                {
                    actions.SetActive(true);
                }
                joustOrResetButton.SetActive(true);
                skipButton.SetActive(false);

                //resolve opponent action
                handleAction("none", opponentAction());
                state = State.ActionDisplay;
                skipped = false; //swap back to false for next time through
            } 
            else
            {
                //foreach loop checking each action card to see if they were selected
                foreach (GameObject action in actionCards)
                {
                    //if selected
                        //show confirmation screen
                            //2 types
                                //color changing
                                    //need to display colors & wait for player to choose one
                                        //then show confirm button & wait for confirmation
                                            //after confirmation handle action -> move to next state
                                //other
                                    //show confirm button & wait for confirmation
                                        //after confirmation handle action -> move to next state

                    if (action.GetComponent<CardDisplay>().selected)
                    {
                        //hide the other action
                        foreach (GameObject otherAction in actionCards)
                        {
                            if (!otherAction.GetComponent<CardDisplay>().selected)
                            {
                                otherAction.SetActive(false);
                            }
                        }
                        //if something is selected remove the skip button
                        skipButton.SetActive(false);
                        //change to big version of card with confirm button next to it
                        if (action.GetComponent<CardDisplay>().displayCard.name == "Heka's Trickery" ||
                            action.GetComponent<CardDisplay>().displayCard.name == "Sekhmet's Stratagem")
                        {

                            //variables for SS
                            bool red = false;
                            bool green = false;
                            bool blue = false;

                            foreach (Card god in playerDeck)
                            {
                                if (god.type == 1 && god.destroyed)
                                {
                                    red = true;
                                }
                                if (god.type == 2 && god.destroyed)
                                {
                                    green = true;
                                }
                                if (god.type == 3 && god.destroyed)
                                {
                                    blue = true;
                                }
                            }
                            foreach (Card god in opponentDeck)
                            {
                                if (god.type == 1 && god.destroyed)
                                {
                                    red = true;
                                }
                                if (god.type == 2 && god.destroyed)
                                {
                                    green = true;
                                }
                                if (god.type == 3 && god.destroyed)
                                {
                                    blue = true;
                                }
                            }

                            resetButton.SetActive(true);
                            foreach (GameObject type in typeChange)
                            {
                                bool selected = false;
                                foreach (GameObject otherTypes in typeChange)
                                {
                                    if (otherTypes.GetComponent<TypeButton>().selected)
                                    {
                                        selected = true;
                                    }
                                }
                                if (selected == false && state == State.ActionSelection)
                                {
                                    type.SetActive(true);
                                }
                                if (action.GetComponent<CardDisplay>().displayCard.name == "Sekhmet's Stratagem")
                                {

                                    if (type.GetComponent<TypeButton>().type == 1 && red == true)
                                    {
                                        type.SetActive(true);
                                    }
                                    else if (type.GetComponent<TypeButton>().type == 2 && green == true)
                                    {
                                        type.SetActive(true);
                                    }
                                    else if (type.GetComponent<TypeButton>().type == 3 && blue == true)
                                    {
                                        type.SetActive(true);
                                    }
                                    else
                                    {
                                        type.SetActive(false);
                                    }
                                }
                                if (type.GetComponent<TypeButton>().selected)
                                {
                                    type.SetActive(true);

                                    if (confirmed)
                                    {
                                        selectedAction = action.GetComponent<CardDisplay>().displayCard;
                                        state = State.Busy;
                                        confirmed = false;
                                        type.GetComponent<TypeButton>().confirmType();
                                        //add place for actions to be presented once chosen
                                        handleAction(action.GetComponent<CardDisplay>().displayCard.name, opponentAction());
                                        action.GetComponent<CardDisplay>().selected = false;
                                        confirmButton.SetActive(false);
                                        resetButton.SetActive(false);
                                        foreach (GameObject types in typeChange)
                                        {
                                            types.GetComponent<TypeButton>().selected = false;
                                            types.SetActive(false);
                                        }
                                        foreach (GameObject actions in displayActionCards)
                                        {
                                            actions.SetActive(true);
                                        }
                                        action.SetActive(false);
                                        joustOrResetButton.SetActive(true);
                                        //if both gods arent destroyed yet continue onto the joust
                                        state = State.ActionDisplay;
                                    }
                                    else
                                    {
                                        //hide the other action
                                        foreach (GameObject otherTypes in typeChange)
                                        {
                                            if (!otherTypes.GetComponent<TypeButton>().selected)
                                            {
                                                otherTypes.SetActive(false);
                                            }
                                        }
                                        confirmButton.SetActive(true);
                                    }
                                }
                            } 
                        }
                        else
                        {
                            confirmButton.SetActive(true);
                            resetButton.SetActive(true);
                            if (confirmed)
                            {
                                selectedAction = action.GetComponent<CardDisplay>().displayCard;
                                state = State.Busy;
                                //add place for actions to be presented once chosen
                                handleAction(action.GetComponent<CardDisplay>().displayCard.name, opponentAction());
                                //remove the skip button & action cards
                                foreach (GameObject actions in actionCards)
                                {
                                    actions.SetActive(false);
                                }
                                foreach (GameObject actions in displayActionCards)
                                {
                                    actions.SetActive(true);
                                }
                                action.SetActive(false);
                                joustOrResetButton.SetActive(true);
                                confirmButton.SetActive(false);
                                resetButton.SetActive(false);
                                action.GetComponent<CardDisplay>().selected = false;
                                state = State.ActionDisplay;
                                confirmed = false;
                            }
                        }
                    }
                }

            }
        }
        else if (state == State.ActionDisplay)
        {
            joustOrResetButton.SetActive(true);
            if (Player.destroyed || Opponent.destroyed)
            {
                joustOrResetButton.GetComponent<Image>().sprite = forNext;
                if (joustOrReset && state == State.ActionDisplay)
                {
                    state = State.Reset;
                    joustOrReset = false;
                }
            }
            else
            {
                joustOrResetButton.GetComponent<Image>().sprite = forJoust;
                if (joustOrReset && state == State.ActionDisplay)
                {
                    state = State.Joust;
                    joustOrReset = false;
                }
            }
        }
        else if (state == State.Joust)
        {
            confirmButton.SetActive(false);
            resetButton.SetActive(false);
            foreach (GameObject type in typeChange)
            {
                type.SetActive(false);
            }
            joustHandler();
            lastGod = Player.type;
            joustOrReset = false;
        }
        else if (state == State.PostJoust)
        {
            postJoustButton.SetActive(true);
            joustOrResetButton.SetActive(false);
            if (postJoust)
            {
                state = State.Reset;
                postJoustButton.SetActive(false);
                foreach (GameObject action in displayActionCards)
                {
                    action.SetActive(false);
                }
                postJoust = false;
            }
        }
        else if (state == State.Reset)
        {
            postJoustButton.SetActive(false);
            foreach (GameObject gods in oGod)
            {
                gods.SetActive(false);
            }
            foreach (GameObject gods in pGod)
            {
                gods.SetActive(false);
            }
            foreach (GameObject action in actionCards)
            {
                action.SetActive(false);
            }
            foreach (GameObject gem in favorGems)
            {
                gem.SetActive(false);
            }
            foreach (GameObject heart in healthIndicator)
            {
                heart.SetActive(false);
            }
            skipButton.SetActive(false);
            foreach (GameObject gods in godDeck)
            {
                gods.SetActive(true);
            }
            foreach (GameObject actions in displayActionCards)
            {
                actions.SetActive(false);
            }
            joustOrResetButton.SetActive(false);
            godType = GameObject.FindGameObjectsWithTag("GodType");
            foreach (GameObject type in godType)
            {
                type.SetActive(false);
            }
            foreach (GameObject types in typeChange)
            {
                types.GetComponent<TypeButton>().selected = false;
                types.SetActive(false);
            }
            Player.resetType();
            Opponent.resetType();
            state = State.GodSelection;
        }
        else if (state == State.GameOver)
        {
            postJoustButton.SetActive(false);
            foreach (GameObject gods in oGod)
            {
                gods.SetActive(false);
            }
            foreach (GameObject gods in pGod)
            {
                gods.SetActive(false);
            }
            foreach (GameObject action in actionCards)
            {
                action.SetActive(false);
            }
            skipButton.SetActive(false);
            foreach (GameObject actions in displayActionCards)
            {
                actions.SetActive(false);
            }
            joustOrResetButton.SetActive(false);
            godType = GameObject.FindGameObjectsWithTag("GodType");
            foreach (GameObject type in godType)
            {
                type.SetActive(false);
            }
            foreach (GameObject types in typeChange)
            {
                types.GetComponent<TypeButton>().selected = false;
                types.SetActive(false);
            }
            foreach (GameObject gem in favorGems)
            {
                gem.SetActive(false);
            }
            foreach (GameObject heart in healthIndicator)
            {
                heart.SetActive(false);
            }
            winLossButton.SetActive(true);
            GameObject text = winLossButton.transform.GetChild(0).gameObject;
            if (playerObj[0].GetComponent<Base>().godsDestroyed == 3)
            {
                text.GetComponent<Text>().text = "YOU LOST";
            }
            else
            {
                text.GetComponent<Text>().text = "YOU WON";
            }
        }
    }

    public void setSkipped()
    {
        skipped = true;
    }

    public void setConfirmed()
    {
        confirmed = true;
    }

    public void setJoustOrReset()
    {
        joustOrReset = true;
    }

    public void setPostJoust()
    {
        postJoust = true;
    }

    public void changeScene()
    {
        SceneManager.LoadScene("MenuScene");
    }

    //remove any selected colors/actions
        //show only the 2 actions and the skip button
    public void resetActionSelection()
    {
        foreach (GameObject action in actionCards)
        {
            action.GetComponent<CardDisplay>().selected = false;
            action.SetActive(true);
        }
        foreach (GameObject types in typeChange)
        {
            types.GetComponent<TypeButton>().selected = false;
            types.SetActive(false);
        }
        skipButton.SetActive(true);
        confirmButton.SetActive(false);
        resetButton.SetActive(false);
    }


    //find and destroy the god from either a joust or an action card destroying it
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
                    playerHealth.GetComponent<PlayerHealth>().updateHealth();
                    Debug.Log("Player Gods Destroyed: " + playerObj[0].GetComponent<Base>().godsDestroyed);
                    if (playerObj[0].GetComponent<Base>().godsDestroyed == 3)
                    {
                        state = State.GameOver;
                    }
                }
                else
                {
                    opponentObj[0].GetComponent<Base>().godsDestroyed++;
                    opponentHealth.GetComponent<OpponentHealth>().updateHealth();
                    Debug.Log("Opponent Gods Destroyed: " + opponentObj[0].GetComponent<Base>().godsDestroyed);
                    if (opponentObj[0].GetComponent<Base>().godsDestroyed == 3)
                    {
                        state = State.GameOver;
                    }
                }
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

    //update the types for the player and opponent to reinforce the god choice
    private void updateTypes()
    {
        //godType[1].GetComponent<GodType>().displayType(true);
        //godType[0].GetComponent<GodType>().displayType(false);
    }

    //takes in the player and opponent's actions
        //displays the actions on the displayActionCards cards
        //if both gods are still alive after the action phase color changes will read the changed type variable and enact the color change
        //the actions are enacted here not in the selection for the player or the god
        //god's colors were changed in the 
    private void handleAction(string player, string opponent)
    {
        Debug.Log("PLAYER ACTION: " + player + " OPPONENT ACTION: " + opponent);
        Debug.Log("PLAYER TYPE: " + Player.type + " OPPONENT TYPE: " + Opponent.type);


        //display actions
        if (player == "none")
        {
            displayActionCards[1].GetComponent<CardDisplay>().displayCard = actionDeck[6];
            displayActionCards[1].GetComponent<CardDisplay>().Display(displayActionCards[1].GetComponent<CardDisplay>().displayCard);
        }
        else
        {
            displayActionCards[1].GetComponent<CardDisplay>().Display(selectedAction);
            godType[0].GetComponent<GodType>().displayType(true);
            pGod[0].GetComponent<CardDisplay>().Display(Player);
            //update player symbol
        }
        
        if (opponent == "none")
        {
            displayActionCards[0].GetComponent<CardDisplay>().displayCard = actionDeck[6];
            displayActionCards[0].GetComponent<CardDisplay>().Display(displayActionCards[0].GetComponent<CardDisplay>().displayCard);
        }
        else
        {
            displayActionCards[0].GetComponent<CardDisplay>().Display(opponentSelectedAction);
            godType[1].GetComponent<GodType>().displayType(false);
            oGod[0].GetComponent<CardDisplay>().Display(Opponent);
            //update opponent symbol
        }

        //all cases where player is destroyed
        if ((player == "Anubis's Judgement" && opponent == "none") ||
            (opponent == "AF" && (player != "Eye of Horus" && player != "Ammit's Feast" && player != "Anubis's Judgement")) ||
            (opponent == "AJ" && (player != "Eye of Horus" && player != "Ammit's Feast" && player != "Anubis's Judgement" && player != "none")))
        {
            destroyGod(playerDeck, Player.name, true);
            pGod[0].GetComponent<CardDisplay>().slash.SetActive(true);
        }

        //all cases where opponent is destroyed
        if ((opponent == "AJ" && player == "none") || 
            (player == "Ammit's Feast" && (opponent != "EH" && opponent != "AF" && opponent != "AJ")) || 
            (player == "Anubis's Judgement" && (opponent != "EH" && opponent != "AF" && opponent != "AJ" && opponent != "none")))
        {
            destroyGod(opponentDeck, Opponent.name, false);
            oGod[0].GetComponent<CardDisplay>().slash.SetActive(true);
        }

        //if player or opponent are not destroyed & one of them chose OS
        if (!Player.destroyed && !Opponent.destroyed && (player == "Osiris's Resurrection" || opponent == "OS"))
        {
            osFlip = true;
        } 
        else
        {
            osFlip = false;
        }

        //if the player or opponent used an action take away a maat
        if (player != "none")
        {
            playerObj[0].GetComponent<Base>().maat--;
            playerFavor.GetComponent<PlayerFavor>().updateFavor();
        }
        if (opponent != "none")
        {
            opponentObj[0].GetComponent<Base>().maat--;
            opponentFavor.GetComponent<OpponentFavor>().updateFavor();
        }


    }


    //Opponent selects/displays their action
        //if at 1 god left use an action always
            //if has 2 or 3 gods left use an action based on a percentage
                //WINNING: 35%  TIED: 60%  LOSING: 85%

    private string opponentAction()
    {
        float action = Random.Range(0.0f, 10.0f);

        if (opponentObj[0].GetComponent<Base>().maat > 0)
        {
            if (opponentObj[0].GetComponent<Base>().godsDestroyed == 2) //always use an action
            {
                return getAction(Opponent.type);
            }
            else //use an action based on the action float + board state
            {
                if (!opponentWinning()) //85% to use an ability
                {
                    if (action < 8.5f) //use an action
                    {
                        return getAction(Opponent.type);
                    }
                    else //skip action
                    {
                        return "none";
                    }
                }
                else if (Player.type == Opponent.type) //60% to use an ability
                {
                    if (action < 6.0f) //use an action
                    {
                        return getAction(Opponent.type);
                    }
                    else //skip action
                    {
                        return "none";
                    }
                }
                else //35% to use an ability
                {
                    if (action < 3.5f) //use an action
                    {
                        return getAction(Opponent.type);
                    }
                    else //skip action
                    {
                        return "none";
                    }
                }
            }
        } 
        else
        {
            return "none";
        }
    }


    //Opponent selects/displays their god choice
        //if first round choose a random god
            //else 75% chance to choose opposite of what player picked last round
            //25% chance to be random
                //random number generated 1-100
    private void opponentGodSelection()
    {
        float temp = Random.Range(0.0f, 10.0f);
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

    //handles joust & consider's if Osiris's Resurrection was played
    private void joustHandler()
    {
        if (Player.type == Opponent.type)
        {
            Debug.Log("Tie");
            state = State.PostJoust;
            return;
        }
        //if OS was played
        if (osFlip)
        {
            if ((Player.type == 3 && Opponent.type == 1) || (Player.type == 1 && Opponent.type == 2) || (Player.type == 2 && Opponent.type == 3))
            {
                Debug.Log("Opponent Wins");
                //need to destroy the copy in their deck not the one that is being referenced
                destroyGod(playerDeck, Player.name, true);
                state = State.PostJoust;
                pGod[0].GetComponent<CardDisplay>().slash.SetActive(true);
                return;
            }
            else
            {
                Debug.Log("Player Wins");
                //need to destroy the copy in their deck not the one that is being referenced
                destroyGod(opponentDeck, Opponent.name, false);
                state = State.PostJoust;
                oGod[0].GetComponent<CardDisplay>().slash.SetActive(true);
                return;
            }
        }
        else
        {
            if ((Player.type == 3 && Opponent.type == 1) || (Player.type == 1 && Opponent.type == 2) || (Player.type == 2 && Opponent.type == 3))
            {
                Debug.Log("Player Wins");
                //need to destroy the copy in their deck not the one that is being referenced
                destroyGod(opponentDeck, Opponent.name, false);
                state = State.PostJoust;
                oGod[0].GetComponent<CardDisplay>().slash.SetActive(true);
                return;
            }
            else
            {
                Debug.Log("Opponent Wins");
                //need to destroy the copy in their deck not the one that is being referenced
                destroyGod(playerDeck, Player.name, true);
                state = State.PostJoust;
                pGod[0].GetComponent<CardDisplay>().slash.SetActive(true);
                return;
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


    private bool checkDead(int type)
    {
        foreach (Card god in opponentDeck)
        {
            if (god.type == type && god.destroyed)
            {
                return true;
            }
        }
        foreach (Card god in playerDeck)
        {
            if (god.type == type && god.destroyed)
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
                Opponent = god;
                oGod[0].GetComponent<CardDisplay>().Display(Opponent);
                godType[1].GetComponent<GodType>().displayType(false);
                return;
            }
        }
    }

    private bool opponentWinning()
    {
        if ((Opponent.type == 3 && Player.type == 1) || (Opponent.type == 1 && Player.type == 2) || (Opponent.type == 2 && Player.type == 3)) {
            return true;
        } 
        else
        {
            return false;
        }
    }


    //chart for choosing abilities based on matchups

                //WINNING
                    //red v green
                        //Ammit's Feast
                    //green v blue
                        //Anubis's Judgement
                    //blue v red
                        //25%
                            //Osiris's Resurrection
                        //75%
                            //Eye of Horus

                //TIED
                    //red v red
                        //if there is a blue god that has been destroyed
                            //50%
                                //Ammit's Feast
                            //50%
                                //Sehkmet's Strategem to blue
                        //else
                            //Ammit's Feast
                    //green v green
                        //50%
                            //Heka's Trickery to red
                        //50%
                            //Anubis's Judgement
                    //blue v blue
                        //refuse to take an action cause wouldn't result in a win no matter what

                //LOSING
                    //red v blue
                        //if there is a green god that has been destroyed
                            //50%
                                //Sekhmet’s Stratagem to green
                            //50%
                                //Ammit’s Feast
                        //else
                            //Ammit’s Feast
                    //blue v green
                        //Osiris's Resurrection
                    //green v red
                        //50%
                            //Anubis's Judgement
                        //50%
                            //Heka's Trickery to blue

    //The 6 actions have 2 letter abreviations to represent them
        //Ammit's Feast = AF
        //Sehkmet's Strategem = SS

        //Anubis's Judgement = AJ
        //Heka's Trickery = HT

        //Eye of Horus = EH
        //Osiris's Resurrection = OS

    private string getAction(int type)
    {
        float per = Random.Range(0.0f, 10.0f);
        if (type == 1 && Player.type == 2) //winning && red
        {
            opponentSelectedAction = actionDeck[1];
            return "AF";
        }
        else if (type == 2 && Player.type == 3) //winning && green
        {
            opponentSelectedAction = actionDeck[2];
            return "AJ";
        }
        else if (type == 3 && Player.type == 1) //winning && blue
        {
            if (per > 7.5f) //above 75%
            {
                opponentSelectedAction = actionDeck[5];
                return "OS";
            }
            else //below 75%
            {
                opponentSelectedAction = actionDeck[4];
                return "EH";
            }
        }
        else if (type == 1 && type == Player.type) //tied && red
        {
            if (checkDead(3) && per > 5.0f) //blue god destroyed && above 50%
            {
                opponentSelectedAction = actionDeck[0];
                //change type to blue
                Opponent.type = 3;
                return "SS";
            }
            else //below 50%
            {
                opponentSelectedAction = actionDeck[1];
                return "AF";
            }
        }
        else if (type == 2 && type == Player.type) //tied && green
        {
            if (per > 5.0f) //above 50%
            {
                opponentSelectedAction = actionDeck[2];
                return "AJ";
            }
            else //below 50%
            {
                opponentSelectedAction = actionDeck[3];
                //change type to red
                Opponent.type = 1;
                return "HT";
            }
        }
        else if (type == 3 && type == Player.type) //tied && blue
        {
            return "none";
        }
        else if (type == 1 && Player.type == 3) //losing && red
        {
            if (checkDead(2) && per > 5.0f) //if green god destroyed && above 50%
            {
                opponentSelectedAction = actionDeck[0];
                //change type to green
                Opponent.type = 2;
                return "SS";
            }
            else //below 50%
            {
                opponentSelectedAction = actionDeck[1];
                return "AF";
            }

        }
        else if (type == 2 && Player.type == 1) //losing && green
        {
            if (per > 5.0f) //above 50%
            {
                opponentSelectedAction = actionDeck[2];
                return "AJ";
            }
            else //below 50%
            {
                opponentSelectedAction = actionDeck[3];
                //change type to blue
                Opponent.type = 3;
                return "HT";
            }
        }
        else if (type == 3 && Player.type == 2) //losing && blue
        {
            opponentSelectedAction = actionDeck[5];
            return "OR";
        }
        return "none";
    }

}
