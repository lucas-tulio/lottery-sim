using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TicketBehavior : MonoBehaviour {
    public TextMeshProUGUI timesPlayedText;
    public TextMeshProUGUI timesLostText;
    public TextMeshProUGUI timesWonText;
    public TextMeshProUGUI timePassedText;
    public TextMeshProUGUI totalNumbersHitText;
    public TextMeshProUGUI maxNumbersHitText;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI instructionsText;
    public Button betButton;
    public Button autoBetButton;
    public Button autoBetStopButton;

    private System.Random random = new System.Random();
    private bool[] selected = new bool[Player.maxBetOtions];
    private List<Button> buttons = new List<Button>();
    List<int> allNumbers = new List<int>();
    private bool isAutoBetting = false;

    void Start () {
        UpdateUI(0);
        betButton.interactable = false;
        autoBetButton.interactable = false;
        // Setup bet buttons  
        GameObject[] buttonObjects = GameObject.FindGameObjectsWithTag("BetOptions");
        int count = 1;
        foreach(GameObject buttonObject in buttonObjects) {
            Button b = buttonObject.GetComponent<Button>();
            b.GetComponentInChildren<TextMeshProUGUI>().text = (count < 10) ? "0" + count : "" + count;
            buttons.Add(b);
            count++;
        }

        // Create a list of all numbers
        for (int i = 0; i < Player.maxBetOtions; i++) {
            allNumbers.Add(i);
        }
    }
     
    public void SelectNumber() {
        string buttonName = EventSystem.current.currentSelectedGameObject.name;
        int numberSelected;

        // Get which number has been clicked
        if (!buttonName.Contains("(")) {
            numberSelected = 0;
        } else {
            string parsedString = buttonName.Replace("Button (", "").Replace(")", "");
            numberSelected = Convert.ToInt32(parsedString);
        }

        // Toggle in the selected number array
        if (GetNumbersSelected().Length < Player.maxBets) {
            selected[numberSelected] = !selected[numberSelected];
        } else {
            selected[numberSelected] = false;
        }

        // Enable or disable bet button
        betButton.interactable = (GetNumbersSelected().Length == Player.maxBets);
        autoBetButton.interactable = (GetNumbersSelected().Length == Player.maxBets);
        
        UpdateInstructions();
        UpdateButtonStates();
    }

    private void UpdateButtonStates() {
        for (int i = 0; i < Player.maxBetOtions; i++) {
            if (selected[i]) {
                buttons[i].GetComponent<Image>().color = new Color(0.2f, 0.73f, 0.95f);
            } else {
                buttons[i].GetComponent<Image>().color = new Color(1f, 1f, 1f);
            }
        }
    }

    public int[] GetNumbersSelected() {
        List<int> numbersSelected = new List<int>();
        for (int i = 0; i < selected.Length; i++) {
            if (selected[i]) {
                numbersSelected.Add(i);
            }
        }
        return numbersSelected.ToArray();
    }

    private void UpdateInstructions() {
        if (GetNumbersSelected().Length < Player.maxBets) {
            instructionsText.text = "Choose " + (Player.maxBets - GetNumbersSelected().Length) + " numbers";
        } else {
            instructionsText.text = "Bet!";
        }
    }

    public void Clear() {
        for(int i = 0; i < Player.maxBetOtions; i++) {
            selected[i] = false;
        }
        UpdateButtonStates();
    }

    public void Bet() {
        if (GetNumbersSelected().Length < Player.maxBets) {
            return;
        }

        // Randomize numbers
        int[] winningNumbers = allNumbers.OrderBy(x => random.Next()).Take(Player.maxBets).ToArray<int>();
        Array.Sort(winningNumbers);

        // Check hits
        int[] playerNumbers = GetNumbersSelected();
        List<int> hits = new List<int>();
        for(int i = 0; i < Player.maxBets; i++) {
            for (int j = 0; j < winningNumbers.Length; j++) {
                if (playerNumbers[i] == winningNumbers[j]) {
                    hits.Add(playerNumbers[i]);
                }
            }
        }

        // Update max numbers hit score
        if (hits.Count() > GameData.player.maxNumbersHit) {
            GameData.player.maxNumbersHit = hits.Count();
        }

        // Add player stats
        GameData.player.timesPlayed += 1;

        // Add to player stats
        if (hits.Count() == Player.maxBets) {
            // Victory!
            GameData.player.money += 1000000;
            GameData.player.wins += 1;
        } else {
            // Lost
            GameData.player.losses += 1;
            GameData.player.money -= 1;
        }

        // Update UI
        UpdateUI(hits.Count());
    }

    private void UpdateUI(int hits) {
        timesPlayedText.text = "Times played: " + GameData.player.timesPlayed;
        timesLostText.text = "Times lost: " + GameData.player.losses;
        timesWonText.text = "Times won: " + GameData.player.wins;
        
        int timePassedDivision = GameData.player.timesPlayed / 52;
        string timePassed = "";
        if (timePassedDivision == 0) {
            timePassed = "less than 1 year";
        } else if (timePassedDivision == 1) {
            timePassed = "1 year";
        } else {
            timePassed = timePassedDivision + " years";
        }
        timePassedText.text = "Real life time passed: " + timePassed + "\n(One lottery draw per week)";

        totalNumbersHitText.text = "Total numbers hit: " + hits;
        maxNumbersHitText.text = "Max numbers hit: " + GameData.player.maxNumbersHit;
        moneyText.text = "$ " + GameData.player.money;
    }

    public void StartAutoBet() {
        isAutoBetting = true;
    }

    public void StopAutoBet() {
        isAutoBetting = false;
    }

    void Update() {
        if (!isAutoBetting) {
            return;
        }

        if (GetNumbersSelected().Length == Player.maxBets) {
            Bet();
        }
    }
}
