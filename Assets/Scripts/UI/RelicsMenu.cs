using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System; 
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

    public List<Button> buttons = new List<Button>();

    public GameObject relicMenu;

    public GameObject relicLoc;
    private GameObject relicObj;
    public GameObject relicCam;
    private float curX;
    private float curY;
    private float curZ;

    private bool noRelicsUnlocked = false;

    public String currentProfile;

    private void LoadCollectibles() {
        String collectibleListPath = $"collectibles.json";

        if(PFileUtil.Load<JsonWrapperUtil<Collectible>>(collectibleListPath) == null) {
            return;
        }
        else {
            PFileUtil.Load<JsonWrapperUtil<Collectible>>(collectibleListPath).list.ForEach(c => collectibles.Add(c));
        }
    }

    private void SetRelic(Collectible c) {
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
        
        foreach(Button b in buttons) {
            if(b.name == c.name+" Button"){
                b.GetComponentInChildren<TMP_Text>().color = Color.white;
            }
            else{
                b.GetComponentInChildren<TMP_Text>().color = Color.gray;
            }
        }
    }

    void InitData() {
        collectibles = new List<Collectible>();
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
        Collectible firstShown = null;
        int y = 0;
        foreach(Collectible c in collectibles) {

            GameObject relic = Instantiate(buttonPrefab, relicInitialLocation.transform);
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
                    SetRelic(c);
                });
            }
            else {
                t.color = Color.black;
                Destroy(relic.GetComponent<MainMenuButton>());
            }
                
            y -= 100;
        }
        if(firstShown != null) {SetRelic(firstShown);}
    }
    
    void Start() {
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
        Start();
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
            Start();
        }

        if(relicObj != null) {
            SpinObject();
            RelicScroll();
        }
    }
}
