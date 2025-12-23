using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour {

    public Button Cabin;
    public Button Graveyard;
    public Button Apartment;
    public Button ThePuncture;
    public Button Tutorial;
    public List<Sprite> levelImages;
    public Image currentLevelImage;
    public Image underline;

    public GameObject levelImageLeft;
    public GameObject levelImageRight;

    public Image divider;

    public Image background;

    private String currentLevel;
    public Vector3 backgroundOrigLocation;

    private bool inAnim = false;

    private List<String> levels = new List<String> { "Tutorial", "Cabin", "Graveyard", "Apartment", "The_Puncture" };

    private void SetLevel(String level, bool firstTime = false) {
        if(inAnim) {
            return;
        }
        int currentLevelNum = levels.IndexOf(currentLevel);
        currentLevel = level;
        int newLevelNum = levels.IndexOf(level);

        float moveTime = 0.5f;
        if(firstTime) {moveTime = 0.01f;}

        //Bold the selected level and create a line underneath by using the list of levels
        foreach (String l in levels) {
            GameObject levelText = GameObject.Find(l);
            if (l == level) {
                levelText.GetComponentInChildren<TMP_Text>().fontStyle = FontStyles.Bold;
                //activate underline image
                levelText.GetComponentInChildren<TMP_Text>().color = new Color(255f, 255f, 255f, 1);
                MoveUnderline(levelText, moveTime);
            } else {
                //change text color
                levelText.GetComponentInChildren<TMP_Text>().color = new Color(178/255f, 201/255f, 226/255f, 1);
                levelText.GetComponentInChildren<TMP_Text>().fontStyle = FontStyles.Normal;
            }
        }
        //update level image on initial open
        if(currentLevelImage.sprite == null) {
            currentLevelImage.sprite = levelImages[levels.IndexOf(currentLevel)];
            return;
        }
        bool left = newLevelNum > currentLevelNum;
        StartCoroutine(LevelAnimation(left));
    }

    private void MoveUnderline(GameObject parent, float moveTime=0.5f) {
        underline.transform.DOKill();
        underline.transform.DOMove(new Vector3(parent.transform.position.x, 
        divider.transform.position.y, parent.transform.position.z), moveTime);

        parent.GetComponentInChildren<TMP_Text>().ForceMeshUpdate();
        float newX = parent.GetComponentInChildren<TMP_Text>().GetRenderedValues(true).x;
        underline.rectTransform.DOSizeDelta(new Vector2(newX, underline.rectTransform.sizeDelta.y), moveTime+0.2f);
    }

    private IEnumerator LevelAnimation(bool left) {
        inAnim = true;
        float moveDuration = 0.2f;

        currentLevelImage.transform.DOComplete();

        Vector3 movePos = left ? levelImageRight.transform.position : levelImageLeft.transform.position;
        Vector3 moveFrom = left ? levelImageLeft.transform.position : levelImageRight.transform.position;

        background.transform.DOMove(movePos, moveDuration);
        yield return new WaitForSeconds(moveDuration);
        currentLevelImage.sprite = levelImages[levels.IndexOf(currentLevel)];
        background.transform.position = moveFrom;
        yield return new WaitForSeconds(0.05f);
        background.transform.DOMove(backgroundOrigLocation, moveDuration);
        yield return new WaitForSeconds(moveDuration);
        
        inAnim = false;
        yield break;
    }

    public void ForceFinishAnimations() {
        background.DOKill();
        underline.DOKill();
        StopAllCoroutines();
        currentLevelImage.sprite = levelImages[levels.IndexOf(currentLevel)];
        inAnim = false;
        background.transform.position = backgroundOrigLocation;
        underline.transform.position = 
        new Vector3(GameObject.Find(currentLevel).transform.position.x, divider.transform.position.y, GameObject.Find(currentLevel).transform.position.z);
    }

    public String GetLevel() {
        return currentLevel;
    }

    private void AddOnClick(GameObject button, String level) {
        button.GetComponent<Button>().onClick.AddListener(() => SetLevel(level));
    }

    void Start() {
        foreach (String l in levels) {
            GameObject b = GameObject.Find(l);
            AddOnClick(b, l);
        }
        backgroundOrigLocation = background.transform.position;
        SetLevel("Cabin", true);
    }

    void Update() {
        //pressing Q or E switches the current level selection
        if (Input.GetKeyDown(KeyCode.Q)) {
            int index = levels.IndexOf(currentLevel);
            if (index == 0) {
                SetLevel(levels[levels.Count - 1]);
            } else {
                SetLevel(levels[index - 1]);
            }

        } else if (Input.GetKeyDown(KeyCode.E)) {
            int index = levels.IndexOf(currentLevel);
            if (index == levels.Count - 1) {
                SetLevel(levels[0]);
            } else {
                SetLevel(levels[index + 1]);
            }
        }
        if(background.transform.position != backgroundOrigLocation && !inAnim) {background.transform.position = backgroundOrigLocation;}
        if(underline.rectTransform.sizeDelta.x < 20) {
            MoveUnderline(GameObject.Find(currentLevel), 0.1f);
        }
    }
}
