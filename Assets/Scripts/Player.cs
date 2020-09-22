using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Player : MonoBehaviour {
    // Player info (for game save/load) and other general game settings
    public static int maxBets = 6;
    public static int maxBetOtions = 60;

    public int bets = 0;
    public int losses = 0;
    public int wins = 0;
    public int money = 0;

    public long modifiedAt;
}