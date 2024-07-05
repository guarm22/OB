using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PFileUtil {
    public static void Save<T>(string fileName, T obj) {
        string json = JsonUtility.ToJson(obj);
        string path = $"{Application.persistentDataPath}/{PlayerDataManager.GetProfile()}/{fileName}";

        if(!Directory.Exists($"{Application.persistentDataPath}/{PlayerDataManager.GetProfile()}")) {
            CreateDirectoryForProfile();
        }

        Debug.Log($"Saving to {path}");
        System.IO.File.WriteAllText(path, json);
    }

    public static T Load<T>(string fileName) {
        string path = $"{Application.persistentDataPath}/{PlayerDataManager.GetProfile()}/{fileName}";

        if(!Directory.Exists($"{Application.persistentDataPath}/{PlayerDataManager.GetProfile()}")) {
            CreateDirectoryForProfile();
        }

        if(!File.Exists(path)) {
            return default;
        }

        string json = System.IO.File.ReadAllText(path);
        return JsonUtility.FromJson<T>(json);
    }

    public static string GetDataDirectory() {
        return $"{Application.persistentDataPath}/{PlayerDataManager.GetProfile()}/";
    }

    private static void CreateDirectoryForProfile() {
        string path = $"{Application.persistentDataPath}/{PlayerDataManager.GetProfile()}/";
        Directory.CreateDirectory(path);
    }
}
