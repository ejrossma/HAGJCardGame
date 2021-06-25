using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
    //All elements that the player and opponent share

    //information that needs to be accessible by other scripts
    public Card[] godDeck;
    public int maat;
    public bool loser;

    //self tracked & if = 3 then change loser to true
    public int godsDestroyed;
}
