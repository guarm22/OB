using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PFileUtil {
    public static List<String> currentFiles = new List<string> {
        "achievementList.json",
        "playerData.json",
        "keybinds.json",
        "collectibles.json",
    };

    public static void ResetFiles() {
        Debug.Log("Resetting files");
        foreach(string file in currentFiles) {
            string path = $"{Application.persistentDataPath}/{PlayerDataManager.GetProfile()}/{file}";
            if(File.Exists(path)) {
                File.Delete(path);
            }
        }
    }

    public static void Save<T>(string fileName, T obj) {
        string json = JsonUtility.ToJson(obj);
        json = EncryptUtil.Encrypt(json, "ThePuncturePassword1234512345123");
        string path = $"{Application.persistentDataPath}/{PlayerDataManager.GetProfile()}/{fileName}";

        if(!Directory.Exists($"{Application.persistentDataPath}/{PlayerDataManager.GetProfile()}")) {
            CreateDirectoryForProfile();
        }

        //Debug.Log($"Saving to {path}");
        System.IO.File.WriteAllText(path, json);
    }

    public static T Load<T>(string fileName) {
        string path = $"{Application.persistentDataPath}/{PlayerDataManager.GetProfile()}/{fileName}";

        if(!Directory.Exists($"{Application.persistentDataPath}/{PlayerDataManager.GetProfile()}")) {
            CreateDirectoryForProfile();
        }

        if(!File.Exists(path)) {
            Debug.Log($"File {path} does not exist");
            return default;
        }

        string json = System.IO.File.ReadAllText(path);
        string newJson = EncryptUtil.Decrypt(json, "ThePuncturePassword1234512345123");
        return JsonUtility.FromJson<T>(newJson);
    }

    public static string GetDataDirectory() {
        return $"{Application.persistentDataPath}/{PlayerDataManager.GetProfile()}/";
    }

    private static void CreateDirectoryForProfile() {
        string path = $"{Application.persistentDataPath}/{PlayerDataManager.GetProfile()}/";
        Directory.CreateDirectory(path);
    }
}
