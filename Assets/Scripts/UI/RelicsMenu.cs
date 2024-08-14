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
    public Image image;

    private int pnum = 1;
    public Button nextButton;
    public Button prevButton;
    public TMP_Text pageText;

    public GameObject noRelicsText;

    public GameObject buttonPrefab;

    public GameObject devStuff;
    public Button hardResetCollectibles;
    public Button softResetCollectibles;
    public Button unlockAllCollectibles;

    public GameObject relicMenu;

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

    private void ShowPage(int page) {
        foreach(Transform child in relicInitialLocation.transform) {
            Destroy(child.gameObject);
        }
        pnum = page;
        Collectible firstShown = null;
        int y = 0;
        for(int i = 0; i < 5; i++) {
            if(collectibles.Count > i + (page - 1) * 5) {
                Collectible c = collectibles[i + (page - 1) * 5];

                GameObject relic = Instantiate(buttonPrefab, relicInitialLocation.transform);
                TMP_Text t = relic.GetComponentInChildren<TMP_Text>();
                relic.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, y);
                t.text = c.name;

                if(c.isCollected) {
                    t.color = Color.white;
                    relic.GetComponent<Button>().onClick.AddListener(() => {
                        SetRelic(c);
                    });
                    if(firstShown == null) {
                        firstShown = c;
                    }
                }
                else {
                    t.color = Color.gray;
                    Destroy(relic.GetComponent<MainMenuButton>());
                }
                
                y -= 100;
            }
        }
        
        SetRelic(firstShown);
        SetPageText();
    }

    private void SetPageText() {
        if(collectibles.Count == 0) {
            pageText.text = "Page 0 of 0";
            return;
        }

        pageText.text = "Page " + pnum + " of " + Mathf.Ceil(collectibles.Count / 5f);
    }
    
    private void SetRelic(Collectible c) {
        if(c == null) {
            description.GetComponent<TMPro.TextMeshProUGUI>().text = "Unlock a relic to see its description.";
            return;
        }

        description.GetComponent<TMPro.TextMeshProUGUI>().text = c.description;
        //image.sprite = c.sprite;
    }

    void InitData() {
        collectibles = new List<Collectible>();
        LoadCollectibles();
        currentProfile = PlayerPrefs.GetString("currentProfile");
        if(collectibles.Count == 0) {
            noRelicsText.SetActive(true);
            relicInitialLocation.SetActive(false);
            image.gameObject.SetActive(false);
            description.SetActive(false);
            return;
        }
        else {
            noRelicsText.SetActive(false);
            relicInitialLocation.SetActive(true);
            image.gameObject.SetActive(true);
            description.SetActive(true);
        }
        ShowPage(1);
    }
    
    void Start() {
        InitData();
        
        hardResetCollectibles.onClick.AddListener(HardResetCollectibles);
        softResetCollectibles.onClick.AddListener(SoftResetCollectibles);
        unlockAllCollectibles.onClick.AddListener(UnlockAllCollectibles);
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
        ShowPage(1);
    }

    private void UnlockAllCollectibles() {
        string collectibleListPath = $"collectibles.json";
        foreach(Collectible c in collectibles) {
            c.isCollected = true;
        }
        PFileUtil.Save(collectibleListPath, new JsonWrapperUtil<Collectible>(collectibles));
        ShowPage(1);
    }

    void Update() {
        //if player presses O
        if(Input.GetKeyDown(KeyCode.O)) {
            devStuff.SetActive(!devStuff.activeSelf);
        }

        if(currentProfile != PlayerPrefs.GetString("currentProfile")) {
            Start();
            SetPageText();
        }
    }
}
