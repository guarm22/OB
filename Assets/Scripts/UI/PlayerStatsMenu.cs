using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Data.Common;

public class PlayerStatsMenu : MonoBehaviour
{
    
    private PlayerData data;
    private int amountOfStats;
    public GameObject statBlockPrefab;
    public GameObject initialLocation;

    private void loadData() {
        data = PFileUtil.Load<PlayerData>("playerData.json");

        if(data == null) {
            Debug.Log("No player data file found, creating new one");
            data = new PlayerData();
            PFileUtil.Save("playerData.json", data);
            loadData();
        }
    }

    private void createStatblock(Stat stat, int index) {
        GameObject statBlock = Instantiate(statBlockPrefab, initialLocation.transform.position, Quaternion.identity, this.transform);
        //get location based on index value
        //every 3rd index, we move down to the next row
        statBlock.transform.position = 
        new Vector3(
            initialLocation.transform.position.x + (index % 3) * 1100, 
            initialLocation.transform.position.y - (index / 3) * 400, 
            initialLocation.transform.position.z);

        //set the text of the statblock
        //get first child, which is the name of the stat
        statBlock.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "Total " + stat.name;
        //get second child, which is the value of the stat
        statBlock.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = stat.value.ToString();
    }

    void Awake() {
        loadData();
        amountOfStats = typeof(PlayerData).GetFields().Length;
        
        for(int i = 0; i < amountOfStats; i++) {
            createStatblock((Stat)typeof(PlayerData).GetFields()[i].GetValue(data), i);
        }
    }

    void Update() {
        
    }
}
