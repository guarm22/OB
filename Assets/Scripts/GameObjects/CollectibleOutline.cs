using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CollectibleOutline : MonoBehaviour {

    public float distance = 2.8f;

    public GameObject player;
    List<Material> materials = new List<Material>();
    public Material shader;

    private bool hovering = false;
    
    void Start() {
        player = GameObject.Find("Player");
        materials.AddRange(GetComponent<Renderer>().materials);

        if(materials.Count < 2) {
            shader = Resources.Load<Material>("Materials/Outline_MAT");
            materials.Add(shader);
        }

        shader = materials[1];
        materials.RemoveAt(1);
        GetComponent<Renderer>().materials = materials.ToArray();
    }

    void Update() {
        if(hovering && Input.GetKeyDown(KeybindManager.instance.GetKeybind("Interact"))) {
            CollectibleControl.Instance.Collect(gameObject);
            CrosshairControl.Instance.SetObjectInRange(false);
            OnMouseExit();
        }

        else if (hovering && Vector3.Distance(transform.position, player.transform.position) > distance) {
            OnMouseExit();
        }
    }

    void OnMouseOver() {
        if(Vector3.Distance(transform.position, player.transform.position) < distance) {
            CrosshairControl.Instance.SetObjectInRange(true);
            hovering = true;
            PlayerUI.Instance.ChangePrompt("(" + KeybindManager.instance.GetKeybind("Interact").ToString() +") Collect", true);
            GetComponent<Renderer>().materials = new Material[] {materials[0], shader};
        }
    }

    void OnMouseExit() {
        CrosshairControl.Instance.SetObjectInRange(false);
        hovering = false;
        PlayerUI.Instance.ChangePrompt("", false);
        GetComponent<Renderer>().materials = materials.ToArray();
    }
}
