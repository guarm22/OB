using Steamworks;
using UnityEngine;

public class SteamHelper : MonoBehaviour {

    void Start() {
        if(!SteamManager.Initialized) {
            return;
        }
        
    }

    public static void IsAchievementUnlocked(string id) {
        SteamUserStats.GetAchievement(id, out bool unlocked);
        Debug.Log("Achievement " + id + " is unlocked: " + unlocked);
    }

    public static void UnlockAchievement(string id) {
        SteamUserStats.SetAchievement(id);
        SteamUserStats.StoreStats();
    }

    public static void ResetAchievements() {
        SteamUserStats.ResetAllStats(true);
    }

    public static void SetStat(string name, int value) {
        SteamUserStats.SetStat(name, value);
        SteamUserStats.StoreStats();
    }
}
