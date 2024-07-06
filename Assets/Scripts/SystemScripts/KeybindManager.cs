using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeybindManager : MonoBehaviour {

    public static KeybindManager instance;
    public List<Keybind> keybinds;

    public void SaveKeybinds() {
        PFileUtil.Save("keybinds.json", new JsonWrapperUtil<Keybind>(keybinds));
    }

    public void LoadKeybinds() {
        if(PFileUtil.Load<JsonWrapperUtil<Keybind>>("keybinds.json") == null) {
            keybinds = GetDefaultKeybinds();
            SaveKeybinds();
        }
        else {
            keybinds = PFileUtil.Load<JsonWrapperUtil<Keybind>>("keybinds.json").list;
        }
    }

    public KeyCode GetKeybind(string action) {
        foreach(Keybind k in keybinds) {
            if(k.action == action) {
                return k.key;
            }
        }
        return KeyCode.None;
    }

    public List<Keybind> GetDefaultKeybinds() {
        keybinds = new List<Keybind>
        {
            new Keybind("Forward", KeyCode.W),
            new Keybind("Back", KeyCode.S),
            new Keybind("Left", KeyCode.A),
            new Keybind("Right", KeyCode.D),
            new Keybind("Jump", KeyCode.Space),
            new Keybind("Crouch", KeyCode.LeftControl),
            new Keybind("Sprint", KeyCode.LeftShift),
            new Keybind("Interact", KeyCode.E),
            new Keybind("Pause", KeyCode.Q),
            new Keybind("Report Menu", KeyCode.Tab),
            new Keybind("Flashlight", KeyCode.F),
            new Keybind("Zoom", KeyCode.Mouse1)
        };
        return keybinds;
    }

    private void Awake() {
        instance = this;
        LoadKeybinds();
    }    

}

[System.Serializable]
public class Keybind {
    public string action;
    public KeyCode key;

    public Keybind(string name, KeyCode key) {
        this.action = name;
        this.key = key;
    }
}