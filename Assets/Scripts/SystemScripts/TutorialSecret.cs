using System.Collections.Generic;
using UnityEngine;

public class TutorialSecret : MonoBehaviour {

    public List<GameObject> rocks;

    void Start() {
        
    }

    void Update() {
        if(Input.GetKeyDown(KeybindManager.instance.GetKeybind("Interact")) || Input.GetKeyDown(KeyCode.Mouse0)) {
            foreach(GameObject rock in rocks) {
                if(rock.activeInHierarchy && rock.GetComponent<Outliner>().hovering) {
                    PlayerUI.Instance.StartCoroutine(PlayerUI.Instance.Acquisition(rock.name, "It's a rock."));
                    rock.SetActive(false);
                    break;
                }
            }
        }
    }
}
