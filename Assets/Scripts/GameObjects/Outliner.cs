using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outliner : MonoBehaviour {
    public float distance = 2.8f;
    public GameObject player;
    List<Material> materials = new List<Material>();
    public Material shader;
    [HideInInspector]
    public bool hovering = false;
    
    void Start() {
        player = GameObject.Find("Player");
        if(shader == null) {
            shader = Resources.Load<Material>("Materials/Outline_MAT");
        }
        materials.AddRange(GetComponent<Renderer>().materials);
    }

    void Update() {
        if(hovering && (Input.GetKeyDown(KeybindManager.instance.GetKeybind("Interact"))|| Input.GetKeyDown(KeyCode.Mouse0))) {
            //OnMouseExit();
        }

        else if (hovering && Vector3.Distance(transform.position, player.transform.position) > distance) {
            OnMouseExit();
        }
    }

    void OnMouseOver() {
        if(Vector3.Distance(transform.position, player.transform.position) < distance) {
            hovering = true;
            //PlayerUI.Instance.ChangePrompt("(" + KeybindManager.instance.GetKeybind("Interact").ToString() +") Collect", true);
            GetComponent<Renderer>().materials = new Material[] {materials[0], shader};
        }
    }

    void OnMouseExit() {
        hovering = false;
        //PlayerUI.Instance.ChangePrompt("", false);
        GetComponent<Renderer>().materials = materials.ToArray();
    }
}
