using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollectibleControl : MonoBehaviour {
    List<Collectible> collectibles = new List<Collectible>();
    List<GameObject> collectiblesOnMap = new List<GameObject>();

    public static CollectibleControl Instance;

    private string collectibleListPath;

    void Start() {
        Instance = this;
        collectibleListPath = $"collectibles.json";

        if(PFileUtil.Load<JsonWrapperUtil<Collectible>>(collectibleListPath) == null) {
            Debug.Log("No collectibles file found, creating new one");
            Save();
        }
        else {
            PFileUtil.Load<JsonWrapperUtil<Collectible>>(collectibleListPath).list.ForEach(c => collectibles.Add(c));
        }

        collectiblesOnMap.AddRange(GameObject.FindGameObjectsWithTag("Collectible"));

        int l = collectibles.Count;
        Debug.Log($"Loaded {l} collectibles from file. There are {collectiblesOnMap.Count} collectibles on map.");
        foreach(GameObject mapC in collectiblesOnMap) {

            //check if collectible from file exists on map
            if (collectibles.Exists(c => c.name == mapC.name)) {
                Collectible c = collectibles.Find(col => col.name == mapC.name);

                //this is for development purposes, to update descriptions if they change
                UpdateCollectible(c, mapC);

                //collectible previously loaded into list
                if(c.isCollected) {
                    mapC.SetActive(false);
                }
            }
            else {
                //first time collectible is being loaded
                Debug.Log("Adding new collectible " + mapC.name);
                string desc = mapC.GetComponent<CollectibleData>().description;
                Collectible newCollectible = new Collectible(mapC.name, desc, false, SceneManager.GetActiveScene().name);
                collectibles.Add(newCollectible);
            }

            //if the object doesnt have the outline script, add it
            if(mapC.GetComponent<CollectibleOutline>() == null) {
                mapC.AddComponent<CollectibleOutline>();
            }
        }

        //check through the list of collectibles to see if any are no longer on the map
        List<Collectible> toRemove = new List<Collectible>();
        foreach(Collectible c in collectibles) {
            //for each collectible c, see if there is a gameobject with a matching name
            if(!collectiblesOnMap.Exists(mc => mc.name == c.name) && c.map == SceneManager.GetActiveScene().name) {
                toRemove.Add(c);
                Debug.Log($"Removing collectible {c.name} from list, no longer on map");
            }
        }
        foreach(Collectible c in toRemove) {
            collectibles.Remove(c);
        }
        Save();

        int amountCollected = collectibles.FindAll(c => c.isCollected).Count;
    }

    private void UpdateCollectible(Collectible c, GameObject mapC) {
        c.description = mapC.GetComponent<CollectibleData>().description;
        c.obj = mapC;
        c.name = mapC.name; //in case the name changed
        Save();

    }

    public void Collect(GameObject obj) {
        foreach(Collectible collectible in collectibles) {
            if(collectible.name == obj.name) {
                collectible.isCollected = true;
                PlayerUI.Instance.StartCoroutine(PlayerUI.Instance.Acquisition(collectible.name));
                obj.SetActive(false);
                break;
            }
        }
        Save();
    }

    public int TotalCollected() {
        return collectibles.FindAll(c => c.isCollected).Count;
    }

    private void Save() { 
        PFileUtil.Save(collectibleListPath, new JsonWrapperUtil<Collectible>(collectibles));
    }
}
