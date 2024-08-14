using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Data.Common;
using UnityEngine.UI;
using TMPro;
using System;

public class PlayerStatsMenu : MonoBehaviour
{
    
    private PlayerData data;
    public GameObject statBlockPrefab;
    public GameObject initialLocation;
    public GameObject statList;
    public List<Stat> stats = new List<Stat>();
    private int statsPerPage = 9;

    public Button nextButton;
    public Button prevButton;
    public TMP_Text pageText;
    private int pnum = 1;

    public String currentProfile;


    private void loadData() {
        data = PFileUtil.Load<PlayerData>("playerData.json");

        if(data == null) {
            Debug.Log("No player data file found, creating new one");
            data = new PlayerData();
            PFileUtil.Save("playerData.json", data);
            loadData();
        }
    }

    private void ShowPage(int num) {
        foreach(Transform child in statList.transform) {
            Destroy(child.gameObject);
        }

        for(int i = 0; i < statsPerPage; i++) {
            if(stats.Count > i + (num - 1) * statsPerPage) {
                Stat stat = stats[i + (num - 1) * statsPerPage];
                GameObject statBlock = Instantiate(statBlockPrefab, initialLocation.transform.position, Quaternion.identity, this.transform);
                //get location based on index value
                //every 3rd index, we move down to the next row
                statBlock.transform.position = 
                new Vector3(
                    initialLocation.transform.position.x + (i % 3) * (Display.main.systemWidth / 3.5f), 
                    initialLocation.transform.position.y - (i / 3) * (Display.main.systemHeight / 5f), 
                    initialLocation.transform.position.z);

                //set the text of the statblock
                //get first child, which is the name of the stat
                statBlock.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "Total " + stat.name;
                //get second child, which is the value of the stat
                statBlock.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = stat.value.ToString() + " " + stat.stringRep;     
                statBlock.transform.SetParent(statList.transform);      
            }
        }
        SetPageText();
    }

    private void SetPageText() {
        if(stats.Count == 0) {
            pageText.text = "Page 0 of 0";
            return;
        }

        pageText.text = "Page " + pnum + " of " + Mathf.Ceil(stats.Count / statsPerPage);
    }

    void InitData() {
        loadData();
        currentProfile = PlayerPrefs.GetString("currentProfile");
        int amountOfStats = typeof(PlayerData).GetFields().Length;
        for(int i = 0; i < amountOfStats; i++) {
            //add to stats list
            stats.Add((Stat)typeof(PlayerData).GetFields()[i].GetValue(data));
        }
        ShowPage(pnum);
    }

    void Awake() {
        InitData();

        nextButton.onClick.AddListener(() => {
            if(stats.Count > pnum * statsPerPage) {
                pnum++;
                ShowPage(pnum);
            }
        });

        prevButton.onClick.AddListener(() => {
            if(pnum > 1) {
                pnum--;
                ShowPage(pnum);
            }
        });
    }

    void Update() {
        if(currentProfile != PlayerPrefs.GetString("currentProfile")) {
            stats.Clear();
            InitData();
            SetPageText();
            currentProfile = PlayerPrefs.GetString("currentProfile");
        }
    }
}
