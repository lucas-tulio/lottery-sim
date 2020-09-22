using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TicketBehavior : MonoBehaviour {
    private System.Random random = new System.Random();

    private bool[] selected = new bool[Player.maxBetOtions];
    private List<Button> buttons = new List<Button>();
    List<int> allNumbers = new List<int>();

    void Start () {
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

    public void Clear() {
        for(int i = 0; i < Player.maxBetOtions; i++) {
            selected[i] = false;
        }
        UpdateButtonStates();
    }

    public void Bet() {
        if (GetNumbersSelected().Length < 6) {
            Debug.Log("pick 6 numbers!");
            return; // TODO: give feedback
        }

        // Randomize numbers
        
        int[] winningNumbers = allNumbers.OrderBy(x => random.Next()).Take(Player.maxBets).ToArray<int>();
        Array.Sort(winningNumbers);
        foreach(int n in winningNumbers) {
            Debug.Log("winnin number " + n);
        }

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

        foreach(int hit in hits) {
            Debug.Log("hit " + hit);
        }
        Debug.Log("" + hits.Count + " numbers hit");
    }
}
