using System;

public class ReportGlitch : CustomDivergence {
    private String roomName;
    private bool active;

    void Awake() {
        active = false;
    }

    public override void DoDivergenceAction(bool enable, DynamicObject obj) {
        roomName = obj.Room;
        if(enable) {
            PlayerUI.Instance.isGlitching = true;
            active = true;
        }
        else {
            active = false;
            PlayerUI.Instance.isGlitching = false;
        }
    }

    void Update() {
        if(active == false) {
            return;
        }
        if(PlayerUI.Instance.isGlitching && PlayerUI.Instance.GetCurrentRoom() != roomName) {
            PlayerUI.Instance.isGlitching = false;
        }
        else if(!PlayerUI.Instance.isGlitching && PlayerUI.Instance.GetCurrentRoom() == roomName) {
            PlayerUI.Instance.isGlitching = true;
        }
    }

}
