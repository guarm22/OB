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
        Debug.Log($"Loaded {l} collectibles from file");
        foreach(GameObject mapC in collectiblesOnMap) {
            if (collectibles.Exists(c => c.name == mapC.name)) {
                Collectible c = collectibles.Find(col => col.name == mapC.name);
                //collectible previously loaded into list
                if(c.isCollected) {
                    mapC.SetActive(false);
                }
            }
            else {
                //first time collectible is being loaded
                string desc = mapC.GetComponent<CollectibleData>().description;
                Collectible newCollectible = new Collectible(mapC.name, desc, false, SceneManager.GetActiveScene().name);
                collectibles.Add(newCollectible);
            }
        }

        //if we've added new collectibles, save the file so we don't have to do it again
        if(l < collectibles.Count) {
            Debug.Log($"{collectibles.Count-l} New collectibles found, saving file");
            Save();
        }

        int amountCollected = collectibles.FindAll(c => c.isCollected).Count;
    }

    public void Collect(GameObject obj) {
        foreach(Collectible collectible in collectibles) {
            if(collectible.name == obj.name) {
                collectible.isCollected = true;
                obj.SetActive(false);
                break;
            }
        }
        Save();
    }

    private void Save() { 
        PFileUtil.Save(collectibleListPath, new JsonWrapperUtil<Collectible>(collectibles));
    }
}
