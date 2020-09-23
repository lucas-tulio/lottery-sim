using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SaveController : MonoBehaviour {
    public TextMeshProUGUI saveMessage;
    public string defaultSaveMessage = "Game is saved automatically every 100 game years";
    
    public void SaveGame() {
        GameData.SaveGame();
        saveMessage.text = "Game saved!";
        StartCoroutine(DisplaySaveFeedback());
    }

    IEnumerator DisplaySaveFeedback() {
        yield return new WaitForSeconds(1.5f);
        saveMessage.text = "Game is saved automatically every 100 game years";
    }
}
