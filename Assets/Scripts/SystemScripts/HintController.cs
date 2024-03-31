using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HintController : MonoBehaviour
{
    public float hintTimer = 10f;
    private float timer;
    public float hintChance = 40f;
    public float hintDelay = 1.5f;
    public float timeBeforeHint = 60f;
    public AudioClip hintSound;

    private IEnumerator findHint() {
        List<DynamicObject> divs = new List<DynamicObject>(GameSystem.Instance.Anomalies);
        foreach(DynamicObject div in divs.Where(div => Time.time - div.divTime > timeBeforeHint && div.divTime > 0f)) {
            if(Random.Range(0, 100) < hintChance) {
                Debug.Log("Hinting");
                doHint(div);
                yield return new WaitForSeconds(hintDelay);
            }
        }
    }

    private void doHint(DynamicObject obj) {
        //AudioSource.PlayClipAtPoint(hintSound, obj.Obj.transform.position);
        LightControl.Instance.FlickerRoomLights(obj.Room, 0.15f);
    }

    // Update is called once per frame
    void Update() {
        if(PlayerUI.paused || GameSystem.Instance.GameOver) {
            return;
        }

        timer += Time.deltaTime;
        if(timer >= hintTimer) {
            timer = 0;
            StartCoroutine(findHint());
        }
    }

    void Start() {
        if(PlayerPrefs.GetInt("HintsEnabled", 1) == 0) {
            //disable script
            this.enabled = false;
        }
    }
}
