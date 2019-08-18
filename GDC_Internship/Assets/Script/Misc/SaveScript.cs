using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveSystem
{
    public static readonly string SaveName = "save.txt";
    public static readonly string SavePath = Application.dataPath +"/Saves/";

    public static void init()
    {
        if (!Directory.Exists(SavePath))
        {
            Debug.Log("PATH NOT FOUND");
            Directory.CreateDirectory(SavePath);
            SaveState NewState = new SaveState();
            NewState.init();
            SaveGame(NewState);
        }
        else
        {
            if (LoadGame() == null)
            {
                Debug.Log("SAVE NOT FOUND");
                SaveState NewState = new SaveState();
                NewState.init();
                SaveGame(NewState);
            }
            else
            {
                Debug.Log("SAVE FOUND");
            }
        }
    }
    public static void SaveGame(SaveState SaveData)
    {
        File.WriteAllText(SavePath + SaveName, JsonUtility.ToJson(SaveData,true));
        Debug.Log("SAVE SUCCESS");
    }
    public static SaveState LoadGame()
    {
        if (File.Exists(SavePath + SaveName))
        {
            Debug.Log("SAVE LOADED");
            return JsonUtility.FromJson<SaveState>(File.ReadAllText(SavePath + SaveName));
        }
        else
        {
            return null;
        }
    }
    public static void ResetGame()
    {
        SaveState NewState = new SaveState();
        NewState.init();
        SaveGame(NewState);
    }
}


