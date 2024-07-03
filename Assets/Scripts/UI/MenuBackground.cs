using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBackground : MonoBehaviour
{
    public Camera mainCamera;

    public List<GameObject> worlds;

    private void rotateCam() {
        mainCamera.transform.Rotate(Vector3.up, 0.01f);
    }

    void Start() {
        //choose a random world to activate
        int randomWorld = Random.Range(0, worlds.Count);
        worlds[randomWorld].SetActive(true);

        //find all gameobjects with "sp" tag and choose a random one
        //GameObject[] sp = GameObject.FindGameObjectsWithTag("sp");
        //if(sp.Length == 0) {return;}
        //int randomSP = Random.Range(0, sp.Length);
        //move the camera to that spawnpoint
       // mainCamera.transform.position = sp[randomSP].transform.position;
        
    }

    // Update is called once per frame
    void Update() {
        rotateCam();
    }
}
