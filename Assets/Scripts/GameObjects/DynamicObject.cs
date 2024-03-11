using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public enum ANOMALY_TYPE {
    NONE,
    Disappearance,
    Replacement,
    Movement,
    Creature,
    Audio,
};

public class DynamicObject {
    public DynamicData data;
    public string Room {get; set;}
    public string Name {get; set;}
    public bool normal;

    public GameObject Obj {get; set;}

    public Vector3 originalPosition {get; set;}

    public DynamicObject(DynamicData data, string room, string name, GameObject obj) {
        this.data = data;
        Name = name;
        Room = room;
        Obj = obj;
        normal = true;
        originalPosition = obj.transform.position;
    }

    public int DoAnomalyAction(bool enable) {
        Debug.Log("Anomaly on " + this.Name + " in " + this.Room + " of type " + AnomalyTypeToString(this.data.type) + 
        (enable ? " ENABLED" : " DISABLED"));
        this.normal = !enable;
        this.data.beenKilled = false;

        if(Obj.GetComponent<CustomDivergence>() != null) {
            Obj.GetComponent<CustomDivergence>().DoDivergenceAction(enable, this);
            return 1;
        }

        switch(this.data.type) {
            case ANOMALY_TYPE.Disappearance:
                this.ObjectDisappearance(enable);
                break;
            
            case ANOMALY_TYPE.Replacement:
                this.ObjectChange(enable);
                break;

            case ANOMALY_TYPE.Creature:
                //done through custom class
                break;

            case ANOMALY_TYPE.Audio:
                this.Audio(enable);
                break;

            case ANOMALY_TYPE.Movement:
                //done through custom class
                break;
        }
        return 1;
    }

//Makes an object disappear or reappear based on the enable argument
    private void ObjectDisappearance(bool enable) {
        if(enable) {
          Vector3 vec = new Vector3(this.Obj.transform.position.x, this.Obj.transform.position.y-100f, this.Obj.transform.position.z);
            this.Obj.transform.position = vec;
        }

        else {
            Vector3 vec = new Vector3(this.Obj.transform.position.x, this.Obj.transform.position.y+100f, this.Obj.transform.position.z);
            this.Obj.transform.position = vec;
        }
    }
    private void Audio(bool enable) {
        if(enable) {
            this.Obj.GetComponent<PlayAudio>().enabled = true;
        }
        else {
            this.Obj.GetComponent<PlayAudio>().enabled = false;
        }
        //footstep sounds
    }

    private void ObjectChange(bool enable) {
        Transform replacement = this.Obj.transform.GetChild(0);
        Vector3 old = replacement.position;
        replacement.localPosition = new Vector3(0, enable ? 10f : -10f, 0);
        this.Obj.transform.position = old; 
    }

    public static List<string> GetAllAnomalyTypes() {
        List<string> res = new List<string>
        {
            "Disappearance",
            "Replacement",
            "Creature",
            "Audio",
            "Movement"
        };
        return res;
    }

    public static string AnomalyTypeToString(ANOMALY_TYPE type) {
        switch(type) {
            case ANOMALY_TYPE.NONE:
                return "NONE";
            case ANOMALY_TYPE.Disappearance:
                return "Disappearance";
            case ANOMALY_TYPE.Replacement:
                return "Replacement";
            case ANOMALY_TYPE.Creature:
                return "Creature";
            case ANOMALY_TYPE.Audio:
                return "Audio";
            case ANOMALY_TYPE.Movement:
                return "Movement";
            default:
                return "Error";
        }
    }

}