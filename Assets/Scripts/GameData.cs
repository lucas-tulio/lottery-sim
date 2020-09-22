using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameData {

    public static Player player = new Player();

    [RuntimeInitializeOnLoadMethod]
    public static void Startup() {
        LoadGame();
    }
    
    public static void SaveGame() {
        player.modifiedAt = DateTime.UtcNow.Ticks;
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/gamesave.save");
        bf.Serialize(file, player);
        file.Close();
    }

    public static void LoadGame() {
        if (SaveExists()) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Open);
            player = (Player)bf.Deserialize(file);
            file.Close();
        }
    }

    public static void DeleteSaveGame() {
        if (SaveExists()) {
            File.Delete(Application.persistentDataPath + "/gamesave.save");
        }
        PlayerPrefs.DeleteAll();
        player = new Player();
#if UNITY_EDITOR
        
#else
        SceneManager.LoadScene("LoginScene");
#endif
    }

    public static bool SaveExists() {
        return File.Exists(Application.persistentDataPath + "/gamesave.save");
    }
}