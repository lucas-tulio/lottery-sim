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
    public Button clearButton;

    private System.Random random = new System.Random();
    private bool[] selected = new bool[Player.maxBetOtions];
    private List<Button> betNumberButtons = new List<Button>();
    public GameObject resultsPanel;
    public List<Button> resultingNumbers = new List<Button>();
    List<int> availableNumbersToBet = new List<int>();
    private bool isAutoBetting = false;

    void Start () {
        UpdateUI(new List<int>(), new int[0]);
        betButton.interactable = false;
        autoBetButton.interactable = false;
        // Setup bet buttons  
        GameObject[] buttonObjects = GetBetButtonsInOrder();
        int count = 1;
        foreach(GameObject buttonObject in buttonObjects) {
            Button b = buttonObject.GetComponent<Button>();
            b.GetComponentInChildren<TextMeshProUGUI>().text = (count < 10) ? "0" + count : "" + count;
            betNumberButtons.Add(b);
            count++;
        }

        // Create a list of all numbers
        for (int i = 0; i < Player.maxBetOtions; i++) {
            availableNumbersToBet.Add(i);
        }
    }

    private GameObject[] GetBetButtonsInOrder() {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("BetOptions");
        Debug.Log("game objects found " + gameObjects.Length);
        Array.Sort(gameObjects, ((x, y) => {
            if (Convert.ToInt32(x.name) > Convert.ToInt32(y.name)) {
                return 1;
            } else if (Convert.ToInt32(y.name) > Convert.ToInt32(x.name)) {
                return -1;
            } else {
                return 0;
            }
        }));
        Debug.Log("game objects after " + gameObjects.Length);
        return gameObjects;
    }
     
    public void SelectNumber() {
        int numberSelected = Convert.ToInt32(EventSystem.current.currentSelectedGameObject.name);

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
                betNumberButtons[i].GetComponent<Image>().color = new Color(0.2f, 0.73f, 0.95f);
            } else {
                betNumberButtons[i].GetComponent<Image>().color = new Color(1f, 1f, 1f);
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
        clearButton.interactable = GetNumbersSelected().Length != 0;
        if (GetNumbersSelected().Length < Player.maxBets) {
            instructionsText.text = "Choose " + (Player.maxBets - GetNumbersSelected().Length) + " numbers";
            autoBetStopButton.interactable = false;
        } else {
            instructionsText.text = "Bet!";
            autoBetStopButton.interactable = true;
        }
    }

    public void Clear() {
        for(int i = 0; i < Player.maxBetOtions; i++) {
            selected[i] = false;
        }
        clearButton.interactable = false;
        autoBetStopButton.interactable = false;
        betButton.interactable = false;
        autoBetButton.interactable = false;
        UpdateButtonStates();
    }

    public void Bet() {
        if (GetNumbersSelected().Length < Player.maxBets) {
            return;
        }

        // Randomize numbers
        int[] winningNumbers = availableNumbersToBet.OrderBy(x => random.Next()).Take(Player.maxBets).ToArray<int>();
        Array.Sort(winningNumbers);

        List<int> hits = GetHits(winningNumbers);

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
        UpdateUI(hits, winningNumbers);
    }

    private List<int> GetHits(int[] winningNumbers) {
        int[] playerNumbers = GetNumbersSelected();
        List<int> hits = new List<int>();
        for(int i = 0; i < Player.maxBets; i++) {
            for (int j = 0; j < winningNumbers.Length; j++) {
                if (playerNumbers[i] == winningNumbers[j]) {
                    hits.Add(playerNumbers[i]);
                }
            }
        }
        return hits;
    }

    private void UpdateUI(List<int> hits, int[] winningNumbers) {
        timesPlayedText.text = "Times played: " + GameData.player.timesPlayed;
        timesLostText.text = "Times lost: " + GameData.player.losses;
        timesWonText.text = "Times won: " + GameData.player.wins;
        
        long timePassedDivision = GameData.player.timesPlayed / 52;
        string timePassed = "";
        if (timePassedDivision == 0) {
            timePassed = "less than 1 year";
        } else if (timePassedDivision == 1) {
            timePassed = "1 year";
        } else {
            timePassed = timePassedDivision + " years";
        }
        timePassedText.text = "Real life time passed: " + timePassed + "\n(at one lottery draw per week)";

        totalNumbersHitText.text = "Total numbers hit this round: " + hits.Count();
        maxNumbersHitText.text = "Max numbers hit: " + GameData.player.maxNumbersHit;
        moneyText.text = "$ " + GameData.player.money;

        // Toggle results display
        if (isAutoBetting && resultsPanel.activeSelf == false) {
            resultsPanel.SetActive(false);
        } else if (isAutoBetting) {
            // no-op
        } else {
            DisplayResults(hits, winningNumbers);
        }
    }

    private void DisplayResults(List<int> hits, int[] winningNumbers) {
        if (winningNumbers.Length == 0) {
            return;
        }

        // Reset result boxes back to blue
        foreach(Button resultButton in resultingNumbers) {   
            resultButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/blue_button06");
        }

        resultsPanel.SetActive(true);
        int[] numbersSelected = GetNumbersSelected();
        for (int i = 0; i < Player.maxBets; i++) {
            int winningNumber = winningNumbers[i] + 1;
            resultingNumbers[i].GetComponentInChildren<TextMeshProUGUI>().text = (winningNumber < 10) ? "0" + winningNumber : "" + winningNumber;

            foreach(int hit in hits) {
                if (hit == winningNumbers[i]) {
                    // User hit a number
                    resultingNumbers[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/green_button06");
                }
            }
        }
    }

    public void StartAutoBet() {
        isAutoBetting = true;
        autoBetButton.interactable = false;
        betButton.interactable = false;
        clearButton.interactable = false;
        resultsPanel.SetActive(false);
    }

    public void StopAutoBet() {
        isAutoBetting = false;
        betButton.interactable = true;
        clearButton.interactable = true;
        autoBetButton.interactable = true;
    }

    void Update() {
        if (!isAutoBetting) {
            return;
        }

        if (GetNumbersSelected().Length == Player.maxBets) {
            Bet();
        }

        if (GameData.player.timesPlayed % 5200 == 0) {
            GameData.SaveGame();
        }
    }
}
