using Steamworks;
using UnityEngine;

public class SteamHelper : MonoBehaviour {

    public static void IsAchievementUnlocked(string id) {
        if(!SteamManager.Initialized) {
            return;
        }

        SteamUserStats.GetAchievement(id, out bool unlocked);
        Debug.Log("Achievement " + id + " is unlocked: " + unlocked);
    }

    public static void UnlockAchievement(string id) {
        if(!SteamManager.Initialized) {
            return;
        }
        SteamUserStats.SetAchievement(id);
        SteamUserStats.StoreStats();
    }

    public static void ResetAchievements() {
        if(!SteamManager.Initialized) {
            return;
        }
        SteamUserStats.ResetAllStats(true);
    }

    public static void SetStat(string name, int value) {
        if(!SteamManager.Initialized) {
            return;
        }
        SteamUserStats.SetStat(name, value);
        SteamUserStats.StoreStats();
    }
}
