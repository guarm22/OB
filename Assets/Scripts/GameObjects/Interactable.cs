using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{

    public string[] JournalText;

    public int PageAmount;

    public static GameObject GetInteractableByName(string name) {
        return GameObject.Find(name);
    }

    public string[] GetJournalText() {
        return JournalText;
    }

}
