using System.Collections;
using System.Collections.Generic;
using SojaExiles;
using UnityEngine;

public class MenuBackground : MonoBehaviour
{
    public Camera mainCamera;

    public List<GameObject> worlds;

    public GameObject currentWorld = null;

    private void rotateCam() {
        mainCamera.transform.Rotate(Vector3.up, 0.01f);
    }

    void Start() {
        int randomWorld = Random.Range(0, worlds.Count);
        ChangeBackground(worlds[randomWorld]);

        //destroy all other worlds
        /*for(int i = 0; i < worlds.Count; i++) {
            if(i != randomWorld) {
                Destroy(worlds[i]);
            }
        }*/
    }

    private void ChangeBackground(GameObject world)
    {
        //choose a random world to activate
        currentWorld = world;
        world.SetActive(true);
        world.GetComponentInChildren<DynamicData>().gameObject.SetActive(false);

        //find all gameobjects with "sp" tag and choose a random one
        GameObject[] sp = GameObject.FindGameObjectsWithTag("MenuSpawnpoint");
        if(sp.Length == 0) {return;}
        int randomSP = Random.Range(0, sp.Length);
        //move the camera to that spawnpoint
        mainCamera.transform.position = sp[randomSP].transform.position;
    }

    // Update is called once per frame
    void Update() {
        rotateCam();
        if(Input.GetKeyDown(KeyCode.Space)) {
            currentWorld.SetActive(false);
            ChangeBackground(worlds[Random.Range(0, worlds.Count)]);
        }
    }
}
