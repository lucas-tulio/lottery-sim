using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Player : MonoBehaviour {
    // Static game config
    public static int maxBets = 6;
    public static int maxBetOtions = 60;

    // Player data that should be saved
    public int timesPlayed = 0;
    public int losses = 0;
    public int wins = 0;
    public int money = 0;
    public int numbersHit = 0;
    public int maxNumbersHit = 0;

    public long modifiedAt;
}