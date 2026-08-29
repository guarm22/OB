using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenGlitch : CustomDivergence {

    [SerializeField]
    private Image overlay;
    private string roomName;
    private bool active = false;
    private GameObject overlayGameObject;
    private ScreenRedFade srf;
    private bool inRoom = false;

    void Start() {
        if(overlay == null){
            overlay = Resources.Load<Image>("Materials/RedScreenImage");
        }
    }

    void Update() {
        if(!active) { return; }

        if(PlayerUI.Instance.currentRoom == roomName && !inRoom) {
            //nothing
            srf.SetProgress(100, -1);
            inRoom = true;
        }
        else if (inRoom && PlayerUI.Instance.currentRoom != roomName) {
            srf.SetProgress(0, 1.5f);
            inRoom = false;
        }
    }

    public override void DoDivergenceAction(bool enable, DynamicObject obj) {
        roomName = obj.Room;
        if(enable) {
            overlayGameObject = Instantiate(overlay, GameObject.Find("DefaultUIPanel").transform).gameObject;
            overlayGameObject.transform.localPosition = Vector3.zero;
            srf = overlayGameObject.GetComponent<ScreenRedFade>();
            active = true;
        }
        else {
            StartCoroutine(EndGlitch(0.5f));
            active = false;
        }
    }

    private IEnumerator EndGlitch(float time) {
        srf.SetProgress(0, time);
        yield return new WaitForSeconds(time);
        srf = null;
        overlayGameObject = null;
        inRoom = false;
        Destroy(overlayGameObject);
    }
}
