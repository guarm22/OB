using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;
public class RelicsMenu : MonoBehaviour {
    public List<Collectible> collectibles = new List<Collectible>();
    
    public GameObject relicInitialLocation;
    public GameObject description;
    public GameObject noRelicsText;
    public GameObject buttonPrefab;
    public GameObject devStuff;
    public Button hardResetCollectibles;
    public Button softResetCollectibles;
    public Button unlockAllCollectibles;

    public TMP_Text relicTutorial;
    public TMP_Text relicMap;

    public List<Button> buttons = new List<Button>();

    public GameObject relicMenu;

    public GameObject relicLoc;
    private GameObject relicObj;
    public GameObject relicCam;
    private float curX;
    private float curY;
    private float curZ;

    public TMP_Text discoveredText;
    private int totalUnlocked = 0;

    private bool noRelicsUnlocked = false;

    public String currentProfile;

    public List<GameObject> currentList = new List<GameObject>();

    public List<String> sortOptions = new List<String> {
        "All",
        "Tutorial",
        "Campsite",
        "Cabin",
        "Graveyard",
        "Apartment"
    };

    public GameObject sortDropdown;

    private void LoadCollectibles() {
        String collectibleListPath = $"collectibles.json";

        if(PFileUtil.Load<JsonWrapperUtil<Collectible>>(collectibleListPath) == null) {
            return;
        }
        else {
            PFileUtil.Load<JsonWrapperUtil<Collectible>>(collectibleListPath).list.ForEach(c => collectibles.Add(c));
        }
    }

    private void SetRelic(Collectible c, int index) {
        if(c == null) {
            description.GetComponent<TMPro.TextMeshProUGUI>().text = "Unlock a relic to see its description.";
            return;
        }
        if(relicObj != null) {
            Destroy(relicObj);
        }
        GameObject prefab = Resources.Load<GameObject>("Collectibles/"+c.name);
        GameObject newInstance = Instantiate(prefab, relicLoc.transform.position, relicLoc.transform.rotation);
        newInstance.transform.parent = relicLoc.transform;
        newInstance.layer = 11;
        relicObj = newInstance;
        curX = 0;
        curY = 90;
        curZ = 0;
        SetRotation();
        
        description.GetComponent<TMPro.TextMeshProUGUI>().text = c.description;
        relicMap.text = "Relic found on: " + c.map;
        discoveredText.text = "#" + (collectibles.IndexOf(c)+1) + " | Relics Discovered: " + totalUnlocked;
        
        foreach(Button b in buttons) {
            if(b.name == c.name+" Button"){
                b.GetComponentInChildren<TMP_Text>().color = Color.white;
            }
            else{
                b.GetComponentInChildren<TMP_Text>().color = Color.gray;
            }
        }
    }

    void InitSort() {
        sortDropdown.GetComponent<Dropdown>().InitDropdown(sortOptions, "All");
        sortDropdown.GetComponent<TMP_Dropdown>().onValueChanged.AddListener(delegate {
            RenderRelicButtons(sortOptions[sortDropdown.GetComponent<TMP_Dropdown>().value]);
        });
    }

    void InitData() {
        collectibles = new List<Collectible>();
        InitSort();
        LoadCollectibles();
        currentProfile = PlayerPrefs.GetString("currentProfile");
        if(collectibles.Count == 0) {
            noRelicsUnlocked = true;
            relicTutorial.gameObject.SetActive(false);
            noRelicsText.SetActive(true);
            relicInitialLocation.SetActive(false);
            description.SetActive(false);
            return;
        }
        else {
            noRelicsText.SetActive(false);
            relicInitialLocation.SetActive(true);
            description.SetActive(true);
        }
        RenderRelicButtons();
    }

    private void RenderRelicButtons(string filter = "All") {
        //clear current buttons before rendering new ones
        foreach(GameObject g in currentList) {
            Destroy(g);
        }
        currentList.Clear();
        buttons.Clear();
        totalUnlocked = 0;

        Collectible firstShown = null;
        int y = 0;
        int index = 1;
        foreach(Collectible c in collectibles) {
            index = index+1;
            if(!c.isCollected) {
                continue;
            }
            if(filter != "All" && c.map != filter) {
                continue;
            }
            totalUnlocked = totalUnlocked + 1;
            GameObject relic = Instantiate(buttonPrefab, relicInitialLocation.transform);
            currentList.Add(relic);
            relic.name = c.name + " Button";
            TMP_Text t = relic.GetComponentInChildren<TMP_Text>();
            relic.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, y);
            t.text = c.name;
            if(c.isCollected) {
                if(firstShown == null) {firstShown = c;}
                t.color = Color.white;
                Button rb = relic.GetComponent<Button>();
                buttons.Add(rb);
                relic.GetComponent<Button>().onClick.AddListener(() => {
                    SetRelic(c, index);
                });
            }
                
            y -= 100;
        }
        if(firstShown != null) {SetRelic(firstShown, 1);}
    }
    
    void OnEnable() {
        InitData();
        
        hardResetCollectibles.onClick.AddListener(HardResetCollectibles);
        softResetCollectibles.onClick.AddListener(SoftResetCollectibles);
        unlockAllCollectibles.onClick.AddListener(UnlockAllCollectibles);
    }

    private void SpinObject() {   
        //x axis - rotate around axis
        //y axis - spin
        //z axis - towards camera
        float rotationSpeed = 250f * Time.deltaTime;
        if(!Input.GetKey(KeyCode.Mouse0)){
            return;
        }
        //mouse left
        if(Input.GetAxis("Mouse X")<0){
            curY += rotationSpeed;
        }
        //mouse right
        if(Input.GetAxis("Mouse X")>0){
            curY -= rotationSpeed;
        }
        //mouse down
        if(Input.GetAxis("Mouse Y")<0) {
            curZ += rotationSpeed;
        }
        //mouse up
        if(Input.GetAxis("Mouse Y")>0) {
            curZ -= rotationSpeed;
        }
        SetRotation();
    }

    private void SetRotation() {
        relicObj.transform.rotation = Quaternion.Euler(curX, curY, curZ);
    }

    private void RelicScroll() {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if(scroll > 0f && relicCam.transform.position.z < -77.9f) {
            relicCam.transform.position += relicCam.transform.forward * 0.1f;
        }
        else if(scroll < 0f && relicCam.transform.position.z > -79.9f) {
            relicCam.transform.position -= relicCam.transform.forward * 0.1f;
        }
    }

    private void HardResetCollectibles() {
        string collectibleListPath = $"collectibles.json";
        collectibles = new List<Collectible>();
        PFileUtil.Save(collectibleListPath, new JsonWrapperUtil<Collectible>(collectibles));
        OnEnable();
    }

    private void SoftResetCollectibles() {
        string collectibleListPath = $"collectibles.json";
        foreach(Collectible c in collectibles) {
            c.isCollected = false;
        }
        PFileUtil.Save(collectibleListPath, new JsonWrapperUtil<Collectible>(collectibles));
    }

    private void UnlockAllCollectibles() {
        string collectibleListPath = $"collectibles.json";
        foreach(Collectible c in collectibles) {
            c.isCollected = true;
        }
        PFileUtil.Save(collectibleListPath, new JsonWrapperUtil<Collectible>(collectibles));
    }

    void Update() {
        //if player presses O
        if(Input.GetKeyDown(KeyCode.O) && GameSystem.InEditor()) {
            devStuff.SetActive(!devStuff.activeSelf);
        }
        if(Input.GetKeyDown(KeyCode.T)) {
            description.SetActive(!description.activeSelf);
            if(description.activeSelf) {
                relicTutorial.text = "Press T to hide description.";
            }
            else {
                relicTutorial.text = "Press T to show description. Click and drag your mouse to rotate the relic.";
            }
        }

        if(currentProfile != PlayerPrefs.GetString("currentProfile")) {
            OnEnable();
        }

        if(relicObj != null) {
            SpinObject();
            RelicScroll();
        }
    }
}
