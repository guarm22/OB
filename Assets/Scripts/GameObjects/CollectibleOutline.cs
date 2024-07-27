using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CollectibleOutline : MonoBehaviour {

    public float distance = 5;
    public GameObject player;
    List<Material> materials = new List<Material>();
    Material shader;
    
    void Start() {
        player = GameObject.Find("Player");
        materials.AddRange(GetComponent<Renderer>().materials);
        shader = materials[1];
        materials.RemoveAt(1);
        GetComponent<Renderer>().materials = materials.ToArray();
    }

    void Update() {
        
    }

    void OnMouseOver() {
        if(Vector3.Distance(transform.position, player.transform.position) < distance) {
            GetComponent<Renderer>().materials = new Material[] {materials[0], shader};
        }
    }

    void OnMouseExit() {
        GetComponent<Renderer>().materials = materials.ToArray();
    }
}
