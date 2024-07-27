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

        int y = 0;
        for(int i = 0; i < 5; i++) {
            if(collectibles.Count > i + (page - 1) * 5) {
                Collectible c = collectibles[i + (page - 1) * 5];

                GameObject relic = Instantiate(buttonPrefab, relicInitialLocation.transform.parent);
                TMP_Text t = relic.GetComponentInChildren<TMP_Text>();
                relic.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, y);
                t.text = c.name;

                if(c.isCollected) {
                    t.color = Color.white;
                    relic.GetComponent<Button>().onClick.AddListener(() => {
                        SetRelic(c);
                    });
                }
                else {
                    t.color = Color.gray;
                    Destroy(relic.GetComponent<MainMenuButton>());
                }
                
                y -= 100;
            }
        }
        SetRelic(collectibles[(page - 1) * 5]);
        pageText.text = "Page " + page + " of " + Mathf.Ceil(collectibles.Count / 5f);
    }
    
    private void SetRelic(Collectible c) {
        description.GetComponent<TMPro.TextMeshProUGUI>().text = c.description;
        //image.sprite = c.sprite;
    }
    
    void Start() {
        LoadCollectibles();
        if(collectibles.Count == 0) {
            noRelicsText.SetActive(true);
            return;
        }
        ShowPage(1);

    }

    void Update() {
        
    }
}
